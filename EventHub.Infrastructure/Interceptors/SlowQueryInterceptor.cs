

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace EventHub.Infrastructure.Interceptors
{
    public class SlowQueryInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<SlowQueryInterceptor> _logger;

        public SlowQueryInterceptor(ILogger<SlowQueryInterceptor> logger)
        {
            _logger = logger;
        }

        public override ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command, 
            CommandExecutedEventData eventData, 
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Duration.TotalMilliseconds > 200)
            {
                _logger.LogWarning(
                 "SLOW SQL ({Duration} ms): {CommandText}",
                 eventData.Duration.TotalMilliseconds,
                 command.CommandText);
            }

            return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
        }
    }
}
