namespace RateLimiter.Services
{
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    public class LeakingBucketManagementService : BackgroundService
    {
        private const int BucketSize = 5;
        private static TimeSpan Interval = TimeSpan.FromSeconds(1);
        private static ConcurrentQueue<(RequestDelegate, HttpContext)> Queue = new ConcurrentQueue<(RequestDelegate, HttpContext)>();

        public static bool IsQueueFull()
        {
            return Queue.Count == BucketSize;
        }

        public static void Enqueue(RequestDelegate del, HttpContext context)
        {
            Queue.Enqueue((del, context));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(Interval))
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (!Queue.IsEmpty)
                    {
                        (RequestDelegate del, HttpContext context) tuple;
                        if (Queue.TryDequeue(out tuple))
                        {
                            try
                            {
                                await tuple.del(tuple.context);
                            }
                            catch (ObjectDisposedException)
                            {
                                Console.WriteLine("HttpContext disposed of.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Dequeuing failed.");
                        }
                    }
                }
            }
        }
    }
}
