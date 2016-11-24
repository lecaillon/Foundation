using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Storage
{
    /// <summary>
    ///     A command to be executed against a relational database.
    /// </summary>
    public interface IRelationalCommand
    {
        /// <summary>
        ///     Gets the command text to be executed.
        /// </summary>
        string CommandText { get; }

        /// <summary>
        ///     Gets the parameters for the command.
        /// </summary>
        IReadOnlyList<IRelationalParameter> Parameters { get; }

        /// <summary>
        ///     Executes the command with no results.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The number of rows affected. </returns>
        int ExecuteNonQuery(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues);

        /// <summary>
        ///     Asynchronously executes the command with no results.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the number of rows affected.
        /// </returns>
        Task<int> ExecuteNonQueryAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Executes the command with a single scalar result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The result of the command. </returns>
        object ExecuteScalar(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues);

        /// <summary>
        ///     Asynchronously executes the command with a single scalar result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        Task<object> ExecuteScalarAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <returns> The result of the command. </returns>
        RelationalDataReader ExecuteReader(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues);

        /// <summary>
        ///     Asynchronously executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="connection"> The connection to execute against. </param>
        /// <param name="parameterValues"> The values for the parameters. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        Task<RelationalDataReader> ExecuteReaderAsync(IRelationalConnection connection, IReadOnlyDictionary<string, object> parameterValues, CancellationToken cancellationToken = default(CancellationToken));
    }
}
