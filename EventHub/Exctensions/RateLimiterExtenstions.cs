using System.Security.Claims;
using System.Threading.RateLimiting;

namespace EventHub.Web.Exctensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {

                options.AddPolicy("create-event", httpContext =>
      RateLimitPartition.GetSlidingWindowLimiter(
          partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
          factory: _ => new SlidingWindowRateLimiterOptions
          {
              PermitLimit = 3,
              Window = TimeSpan.FromHours(1),
              SegmentsPerWindow = 6,
              QueueLimit = 0
          }));

                options.AddPolicy("send-message", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0
                        }));


                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.WriteAsync
                    ("Too many requests. Pleast try again later.", cancellationToken);
                };
            });

            return services;
        }
    }
}
