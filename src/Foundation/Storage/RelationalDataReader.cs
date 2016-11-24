using System;
using System.Data.Common;
using Foundation.Utilities;

namespace Foundation.Storage
{
    /// <summary>
    ///     Reads result sets from a relational database.
    /// </summary>
    public class RelationalDataReader : IDisposable
    {
        private readonly IRelationalConnection _connection;
        private readonly DbCommand _command;
        private readonly DbDataReader _reader;

        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelationalDataReader" /> class.
        /// </summary>
        /// <param name="connection"> The connection. </param>
        /// <param name="command"> The command that was executed. </param>
        /// <param name="reader"> The underlying reader for the result set. </param>
        public RelationalDataReader(IRelationalConnection connection, DbCommand command, DbDataReader reader)
        {
            Check.NotNull(command, nameof(command));
            Check.NotNull(reader, nameof(reader));

            _connection = connection;
            _command = command;
            _reader = reader;
        }

        /// <summary>
        /// For testing
        /// </summary>
        protected RelationalDataReader()
        {
        }

        /// <summary>
        ///     Gets the underlying reader for the result set.
        /// </summary>
        public virtual DbDataReader DbDataReader => _reader;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _reader.Dispose();
                _command.Dispose();
                _connection?.Close();

                _disposed = true;
            }
        }
    }
}
