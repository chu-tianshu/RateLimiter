namespace RateLimiter.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class TokenBucketManagementService : BackgroundService
    {
        private const int BucketSize = 5;
        private const int TokensRefilledPerSecond = 1;
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);

        public static int RemainingTokenCount { get; private set; } = 0;

        public static void ConsumeOneToken()
        {
            if (RemainingTokenCount == 0)
            {
                return;
            }

            RemainingTokenCount--;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(Interval))
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    if (RemainingTokenCount < BucketSize)
                    {
                        RemainingTokenCount += TokensRefilledPerSecond;
                    }
                }
            }
        }
    }
}
