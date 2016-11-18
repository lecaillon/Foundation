namespace Foundation.Migrations.Operations
{
    public class RenameSequenceOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }

        /// <summary>
        ///     The new schema name or null if unchanged.
        /// </summary>
        public virtual string NewName { get; set; }

        /// <summary>
        ///     The new sequence name or null if unchanged.
        /// </summary>
        public virtual string NewSchema { get; set; }
    }
}
