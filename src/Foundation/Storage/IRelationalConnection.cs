using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Foundation.Storage
{
    /// <summary>
    ///     Represents a connection with a relational database.
    /// </summary>
    public interface IRelationalConnection : IRelationalTransactionManager, IDisposable
    {
        /// <summary>
        ///     Gets the connection string for the database.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        ///     Gets the underlying <see cref="System.Data.Common.DbConnection" /> used to connect to the database.
        /// </summary>
        DbConnection DbConnection { get; }

        /// <summary>
        ///     Gets the timeout for executing a command against the database.
        /// </summary>
        int? CommandTimeout { get; set; }

        /// <summary>
        ///     Opens the connection to the database.
        /// </summary>
        void Open();

        /// <summary>
        ///     Asynchronously opens the connection to the database.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task OpenAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Closes the connection to the database.
        /// </summary>
        void Close();

        /// <summary>
        ///     Gets a value indicating whether the multiple active result sets feature is enabled.
        /// </summary>
        bool IsMultipleActiveResultSetsEnabled { get; }

        /// <summary>
        ///     Gets or sets the active cursor.
        /// </summary>
        //IValueBufferCursor ActiveCursor { get; set; }

        /// <summary>
        ///     Gets the current transaction.
        /// </summary>
        new IDbContextTransaction CurrentTransaction { get; }
    }
}
