
namespace EventHub.Web.Filters
{
using Microsoft.AspNetCore.Mvc.Filters;
    using System.Diagnostics;
    // This filter is used to monitor the performance of the application
    //Can easily detect slow or heavy requests
    
    //TODO: make async later
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
            var elapsed = _stopwatch.Elapsed.TotalMilliseconds;

            if (elapsed > 500)
            {
                _logger.LogError("SLOW REQUEST: {Action} took {Elapsed} ms",
                    actionName,
                    elapsed);
            }
            else if (elapsed > 200)
            {
                _logger.LogWarning("Slow request: {Action} took {Elapsed} ms",
                    actionName,
                    elapsed);
            }
            else
            {
                _logger.LogInformation("{Action} took {Elapsed} ms",
                    actionName,
                    elapsed);
            }
        }

       
    }
}
