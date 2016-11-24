using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Foundation.Utilities;
using Microsoft.Extensions.Logging;

namespace Foundation.Storage
{
    public static class RelationalLoggerExtensions
    {
        public static void LogCommandExecuted(this ILogger logger, DbCommand command, long startTimestamp, long currentTimestamp)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(command, nameof(command));

            if (logger.IsEnabled(LogLevel.Information))
            {
                var logParameterValues = command.Parameters.Count > 0;
                var parameters = command.Parameters.Cast<DbParameter>().Select(p => new DbParameterLogData(p.ParameterName,
                                                                                                           logParameterValues ? p.Value : "?",
                                                                                                           logParameterValues,
                                                                                                           p.Direction,
                                                                                                           p.DbType,
                                                                                                           p.IsNullable,
                                                                                                           p.Size,
                                                                                                           p.Precision,
                                                                                                           p.Scale)).ToList();

                var logData = new DbCommandLogData(command.CommandText.TrimEnd(),
                                                   command.CommandType,
                                                   command.CommandTimeout,
                                                   parameters,
                                                   DeriveTimespan(startTimestamp, currentTimestamp));

                logger.Log(LogLevel.Information, (int)RelationalEventId.ExecutedCommand, logData, null, (state, _) =>
                {
                    var elapsedMilliseconds = DeriveTimespan(startTimestamp, currentTimestamp);

                    return ResX.RelationalLoggerExecutedCommand(string.Format(CultureInfo.InvariantCulture, "{0:N0}", elapsedMilliseconds),
                                                                state.Parameters.Select(p => $"{p.Name}={p.FormatParameter()}").Join(), // Interpolation okay here because value is always a string.
                                                                state.CommandType,
                                                                state.CommandTimeout,
                                                                Environment.NewLine,
                                                                state.CommandText);
                });
            }
        }

        public static void LogDebug(this ILogger logger, RelationalEventId eventId, Func<string> formatter)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.Log<object>(LogLevel.Debug, (int)eventId, null, null, (_, __) => formatter());
            }
        }

        public static void LogDebug<TState>(this ILogger logger, RelationalEventId eventId, TState state, Func<TState, string> formatter)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.Log(LogLevel.Debug, (int)eventId, state, null, (s, __) => formatter(s));
            }
        }

        public static void LogWarning(this ILogger logger, RelationalEventId eventId, Func<string> formatter)
        {
            // Always call Log for Warnings because Warnings as Errors should work even if LogLevel.Warning is not enabled.
            logger.Log<object>(LogLevel.Warning, (int)eventId, eventId, null, (_, __) => formatter());
        }

        public static void LogInformation(this ILogger logger, RelationalEventId eventId, Func<string> formatter)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.Log<object>(LogLevel.Information, (int)eventId, null, null, (_, __) => formatter());
            }
        }

        private static readonly double TimestampToMilliseconds = (double)TimeSpan.TicksPerSecond / (Stopwatch.Frequency * TimeSpan.TicksPerMillisecond);

        private static long DeriveTimespan(long startTimestamp, long currentTimestamp) => (long)((currentTimestamp - startTimestamp) * TimestampToMilliseconds);

        private static string Join(this IEnumerable<object> source, string separator = ", ") => string.Join(separator, source);
    }
}
