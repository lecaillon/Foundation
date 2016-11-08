namespace Foundation.Migrations.Operations
{
    public class DropTableOperation : MigrationOperation
    {
        public DropTableOperation()
        {
            IsDestructiveChange = true;
        }

        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
    }
}
