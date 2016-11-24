using System;

namespace Foundation.Storage
{
    /// <summary>
    ///     A transaction against the database.
    /// </summary>
    public interface IDbContextTransaction : IDisposable
    {
        /// <summary>
        ///     Commits all changes made to the database in the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        ///     Discards all changes made to the database in the current transaction.
        /// </summary>
        void Rollback();
    }
}
