namespace Foundation.Migrations.Operations
{
    public class DropSequenceOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
    }
}
