namespace Foundation.Migrations.Operations
{
    public class RestartSequenceOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual long StartValue { get; set; } = 1L;
    }
}
