namespace Foundation.Migrations.Operations
{
    public class CreateIndexOperation : MigrationOperation
    {
        public virtual bool IsUnique { get; set; }
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
        public virtual string[] Columns { get; set; }
    }
}
