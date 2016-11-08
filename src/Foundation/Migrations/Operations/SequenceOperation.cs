namespace Foundation.Migrations.Operations
{
    public class SequenceOperation : MigrationOperation
    {
        public virtual int IncrementBy { get; set; } = 1;
        public virtual long? MaxValue { get; set; }
        public virtual long? MinValue { get; set; }
        public virtual bool IsCyclic { get; set; }
    }
}
