using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Storage
{
    /// <summary>
    ///     Creates and manages the current transaction for a relational database.
    /// </summary>
    public interface IRelationalTransactionManager : IDbContextTransactionManager
    {
        /// <summary>
        ///     Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel"> The isolation level to use for the transaction. </param>
        /// <returns> The newly created transaction. </returns>
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        ///     Asynchronously begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel"> The isolation level to use for the transaction. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the newly created transaction.
        /// </returns>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Specifies an existing <see cref="DbTransaction" /> to be used for database operations.
        /// </summary>
        /// <param name="transaction"> The transaction to be used. </param>
        /// <returns>
        ///     An instance of <see cref="IDbTransaction" /> that wraps the provided transaction.
        /// </returns>
        IDbContextTransaction UseTransaction(DbTransaction transaction);
    }
}
