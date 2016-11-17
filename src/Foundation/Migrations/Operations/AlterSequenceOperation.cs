using Foundation.Metadata.Annotations;

namespace Foundation.Migrations.Operations
{
    public class AlterSequenceOperation : SequenceOperation, IAlterMigrationOperation
    {
        public virtual string Schema { get; set; }
        public virtual string Name { get; set; }
        public virtual SequenceOperation OldSequence { get; set; } = new SequenceOperation();
        Annotable IAlterMigrationOperation.OldAnnotations => OldSequence;
    }
}
