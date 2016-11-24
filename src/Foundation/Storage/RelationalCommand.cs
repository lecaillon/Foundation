using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Utilities;
using Microsoft.Extensions.Logging;

namespace Foundation.Storage
{
    /// <summary>
    ///     A command to be executed against a relational database.
    /// </summary>
    public class RelationalCommand : IRelationalCommand
    {
        public RelationalCommand(ILogger logger, DiagnosticSource diagnosticSource, string commandText, IReadOnlyList<IRelationalParameter> parameters)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(diagnosticSource, nameof(diagnosticSource));
            Check.NotNull(commandText, nameof(commandText));
            Check.NotNull(parameters, nameof(parameters));

            Logger = logger;
            DiagnosticSource = diagnosticSource;
            CommandText = commandText;
            Parameters = parameters;
        }

        protected virtual ILogger Logger { get; }

        protected virtual DiagnosticSource DiagnosticSource { get; }

        /// <summary>
        ///     Gets the command text to be executed.
        /// </summary>
        public virtual string CommandText { get; }

        /// <summary>
        ///     Gets the parameters for the command.
        /// </summary>
        public virtual IReadOnlyList<IRelationalParameter> Parameters { get; }

        /// <summary>
        ///     Executes the command with no results.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The number of rows affected. </returns>
        public virtual int ExecuteNonQuery(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues)
            => (int)Execute(Check.NotNull(connection, nameof(connection)), nameof(ExecuteNonQuery), parameterValues);

        /// <summary>
        ///     Asynchronously executes the command with no results.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the number of rows affected.
        /// </returns>
        public virtual Task<int> ExecuteNonQueryAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(Check.NotNull(connection, nameof(connection)), nameof(ExecuteNonQuery), parameterValues, cancellationToken: cancellationToken).Cast<object, int>();

        /// <summary>
        ///     Executes the command with a single scalar result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The result of the command. </returns>
        public virtual object ExecuteScalar(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues)
            => Execute(Check.NotNull(connection, nameof(connection)), nameof(ExecuteScalar), parameterValues);

        /// <summary>
        ///     Asynchronously executes the command with a single scalar result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        public virtual Task<object> ExecuteScalarAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(Check.NotNull(connection, nameof(connection)), nameof(ExecuteScalar), parameterValues, cancellationToken: cancellationToken);

        /// <summary>
        ///     Executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The result of the command. </returns>
        public virtual RelationalDataReader ExecuteReader(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues)
            => (RelationalDataReader)Execute(Check.NotNull(connection, nameof(connection)), nameof(ExecuteReader), parameterValues, closeConnection: false);

        /// <summary>
        ///     Asynchronously executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        public virtual Task<RelationalDataReader> ExecuteReaderAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync(Check.NotNull(connection, nameof(connection)), nameof(ExecuteReader), parameterValues, closeConnection: false, cancellationToken: cancellationToken).Cast<object, RelationalDataReader>();

        protected virtual object Execute(IRelationalConnection connection, string executeMethod, IReadOnlyDictionary<string, object> parameterValues, bool closeConnection = true)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));

            var dbCommand = CreateCommand(connection, parameterValues);

            connection.Open();

            var startTimestamp = Stopwatch.GetTimestamp();
            var instanceId = Guid.NewGuid();

            DiagnosticSource.WriteCommandBefore(dbCommand, executeMethod, instanceId, startTimestamp, async: false);

            object result;
            try
            {
                switch (executeMethod)
                {
                    case nameof(ExecuteNonQuery):
                    {
                        using (dbCommand)
                        {
                            result = dbCommand.ExecuteNonQuery();
                        }

                        break;
                    }
                    case nameof(ExecuteScalar):
                    {
                        using (dbCommand)
                        {
                            result = dbCommand.ExecuteScalar();
                        }

                        break;
                    }
                    case nameof(ExecuteReader):
                    {
                        try
                        {
                            result = new RelationalDataReader(connection, dbCommand,dbCommand.ExecuteReader());
                        }
                        catch
                        {
                            dbCommand.Dispose();
                            throw;
                        }

                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }

                var currentTimestamp = Stopwatch.GetTimestamp();

                Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);

                DiagnosticSource.WriteCommandAfter(dbCommand, executeMethod, instanceId, startTimestamp, currentTimestamp);

                if (closeConnection)
                {
                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();

                Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);

                DiagnosticSource.WriteCommandError(dbCommand, executeMethod, instanceId, startTimestamp, currentTimestamp, exception, async: false);

                connection.Close();

                throw;
            }
            finally
            {
                dbCommand.Parameters.Clear();
            }

            return result;
        }

        protected virtual async Task<object> ExecuteAsync(IRelationalConnection connection, string executeMethod, IReadOnlyDictionary<string, object> parameterValues, bool closeConnection = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));

            var dbCommand = CreateCommand(connection, parameterValues);

            await connection.OpenAsync(cancellationToken);

            var startTimestamp = Stopwatch.GetTimestamp();
            var instanceId = Guid.NewGuid();

            DiagnosticSource.WriteCommandBefore(dbCommand, executeMethod, instanceId, startTimestamp, async: true);

            object result;
            try
            {
                switch (executeMethod)
                {
                    case nameof(ExecuteNonQuery):
                    {
                        using (dbCommand)
                        {
                            result = await dbCommand.ExecuteNonQueryAsync(cancellationToken);
                        }

                        break;
                    }
                    case nameof(ExecuteScalar):
                    {
                        using (dbCommand)
                        {
                            result = await dbCommand.ExecuteScalarAsync(cancellationToken);
                        }

                        break;
                    }
                    case nameof(ExecuteReader):
                    {
                        try
                        {
                            result = new RelationalDataReader(connection, dbCommand, await dbCommand.ExecuteReaderAsync(cancellationToken));
                        }
                        catch
                        {
                            dbCommand.Dispose();

                            throw;
                        }

                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }

                var currentTimestamp = Stopwatch.GetTimestamp();

                Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);

                DiagnosticSource.WriteCommandAfter(dbCommand, executeMethod, instanceId, startTimestamp, currentTimestamp, async: true);

                if (closeConnection)
                {
                    connection.Close();
                }
            }
            catch (Exception exception)
            {
                var currentTimestamp = Stopwatch.GetTimestamp();

                Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);

                DiagnosticSource.WriteCommandError(dbCommand, executeMethod, instanceId, startTimestamp, currentTimestamp, exception, async: true);

                connection.Close();

                throw;
            }
            finally
            {
                dbCommand.Parameters.Clear();
            }

            return result;
        }

        private DbCommand CreateCommand(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = connection.DbConnection.CreateCommand();

            command.CommandText = CommandText;

            if (connection.CurrentTransaction != null)
            {
                command.Transaction = connection.CurrentTransaction.GetDbTransaction();
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int)connection.CommandTimeout;
            }

            if (Parameters.Count > 0)
            {
                if (parameterValues == null)
                {
                    throw new InvalidOperationException(ResX.MissingParameterValue(Parameters[0].InvariantName));
                }

                foreach (var parameter in Parameters)
                {
                    object parameterValue;

                    if (parameterValues.TryGetValue(parameter.InvariantName, out parameterValue))
                    {
                        parameter.AddDbParameter(command, parameterValue);
                    }
                    else
                    {
                        throw new InvalidOperationException(ResX.MissingParameterValue(parameter.InvariantName));
                    }
                }
            }

            return command;
        }
    }
}
