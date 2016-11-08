namespace Foundation.Migrations.Operations
{
    public class AlterTableOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
    }
}
