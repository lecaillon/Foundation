namespace Foundation.Migrations.Operations
{
    public class AddColumnOperation : ColumnOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
    }
}
