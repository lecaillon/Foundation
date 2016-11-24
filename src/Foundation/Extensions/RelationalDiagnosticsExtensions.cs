using System;
using System.Data.Common;
using System.Diagnostics;

namespace Foundation
{
    internal static class RelationalDiagnosticsExtensions
    {
        private const string NamePrefix = "Foundation.";

        public const string BeforeExecuteCommand = NamePrefix + nameof(BeforeExecuteCommand);
        public const string AfterExecuteCommand = NamePrefix + nameof(AfterExecuteCommand);
        public const string CommandExecutionError = NamePrefix + nameof(CommandExecutionError);

        public static void WriteCommandBefore(this DiagnosticSource diagnosticSource, DbCommand command, string executeMethod, Guid instanceId, long startTimestamp, bool async)
        {
            if (diagnosticSource.IsEnabled(BeforeExecuteCommand))
            {
                diagnosticSource.Write(BeforeExecuteCommand, new RelationalDiagnosticSourceBeforeMessage
                                                             {
                                                                 Command = command,
                                                                 ExecuteMethod = executeMethod,
                                                                 InstanceId = instanceId,
                                                                 Timestamp = startTimestamp,
                                                                 IsAsync = async
                                                             });
            }
        }

        public static void WriteCommandAfter(this DiagnosticSource diagnosticSource, DbCommand command, string executeMethod, Guid instanceId, long startTimestamp, long currentTimestamp, bool async = false)
        {
            if (diagnosticSource.IsEnabled(AfterExecuteCommand))
            {
                diagnosticSource.Write(AfterExecuteCommand, new RelationalDiagnosticSourceAfterMessage
                                                            {
                                                                Command = command,
                                                                ExecuteMethod = executeMethod,
                                                                InstanceId = instanceId,
                                                                Timestamp = currentTimestamp,
                                                                Duration = currentTimestamp - startTimestamp,
                                                                IsAsync = async
                                                            });
            }
        }

        public static void WriteCommandError(this DiagnosticSource diagnosticSource, DbCommand command, string executeMethod, Guid instanceId, long startTimestamp, long currentTimestamp, Exception exception, bool async)
        {
            if (diagnosticSource.IsEnabled(CommandExecutionError))
            {
                diagnosticSource.Write(CommandExecutionError, new RelationalDiagnosticSourceAfterMessage
                                                              {
                                                                  Command = command,
                                                                  ExecuteMethod = executeMethod,
                                                                  InstanceId = instanceId,
                                                                  Timestamp = currentTimestamp,
                                                                  Duration = currentTimestamp - startTimestamp,
                                                                  Exception = exception,
                                                                  IsAsync = async
                                                              });
            }
        }

        internal class RelationalDiagnosticSourceAfterMessage
        {
            public DbCommand Command { get; set; }
            public string ExecuteMethod { get; set; }
            public bool IsAsync { get; set; }
            public Guid InstanceId { get; set; }
            public long Timestamp { get; set; }
            public long Duration { get; set; }
            public Exception Exception { get; set; }
        }

        internal class RelationalDiagnosticSourceBeforeMessage
        {
            public DbCommand Command { get; set; }
            public string ExecuteMethod { get; set; }
            public bool IsAsync { get; set; }
            public Guid InstanceId { get; set; }
            public long Timestamp { get; set; }
        }
    }

}
