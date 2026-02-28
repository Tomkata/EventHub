
namespace EventHub.Web.Filters
{
using Microsoft.AspNetCore.Mvc.Filters;
    using System.Diagnostics;

    public class PerformanceMonitoringFilter : IActionFilter
    {
        private readonly ILogger<PerformanceMonitoringFilter> _logger;
        private  Stopwatch _stopwatch;

        public PerformanceMonitoringFilter(ILogger<PerformanceMonitoringFilter> logger)
        {
            this._logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"Starting excecution of action: {actionName}");
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var actionName = context.ActionDescriptor.DisplayName;
            var elapsedTimes = _stopwatch.Elapsed.TotalMilliseconds;

            _logger.LogInformation($"Action '{actionName}' executed in {elapsedTimes} ms");
        }

       
    }
}
