namespace RateLimiter.Middlewares
{
    using System.Net;
    using RateLimiter.Services;

    public class LeakingBucketRateLimiterMiddleware
    {
        private readonly RequestDelegate next;

        public LeakingBucketRateLimiterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (LeakingBucketManagementService.IsQueueFull())
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            }
            else
            {
                LeakingBucketManagementService.Enqueue(this.next, context);
            }
        }
    }
}
