namespace Foundation.Migrations.Operations
{
    public class RenameColumnOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
        public virtual string NewName { get; set; }
    }
}
