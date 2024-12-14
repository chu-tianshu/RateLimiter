namespace RateLimiter.Middlewares
{
    using System.Net;
    using RateLimiter.Services;

    public class TokenBucketRateLimiterMiddleware
    {
        private readonly RequestDelegate next;

        public TokenBucketRateLimiterMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (BucketManagementService.RemainingTokenCount > 0)
            {
                BucketManagementService.ConsumeOneToken();
                await this.next(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

                return;
            }
        }
    }
}
