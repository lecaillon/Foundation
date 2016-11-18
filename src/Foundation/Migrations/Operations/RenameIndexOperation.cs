namespace Foundation.Migrations.Operations
{
    public class RenameIndexOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string NewName { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
    }
}
