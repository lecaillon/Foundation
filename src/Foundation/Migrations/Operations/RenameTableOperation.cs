namespace Foundation.Migrations.Operations
{
    public class RenameTableOperation : MigrationOperation
    {
        public virtual string Schema { get; set; }
        public virtual string Name { get; set; }

        /// <summary>
        ///     The new schema name or null if unchanged.
        /// </summary>
        public virtual string NewSchema { get; set; }

        /// <summary>
        ///     The new table name or null if unchanged.
        /// </summary>
        public virtual string NewName { get; set; }
    }
}
