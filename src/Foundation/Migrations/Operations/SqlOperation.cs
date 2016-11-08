namespace Foundation.Migrations.Operations
{
    public class SqlOperation : MigrationOperation
    {
        public virtual string Sql { get; set; }
        public virtual bool SuppressTransaction { get; set; }
    }
}
