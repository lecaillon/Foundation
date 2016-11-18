using Foundation.Metadata.Annotations;

namespace Foundation.Migrations.Operations
{
    public class AlterColumnOperation : ColumnOperation, IAlterMigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
        public virtual ColumnOperation OldColumn { get; set; } = new ColumnOperation();
        Annotable IAlterMigrationOperation.OldAnnotations => OldColumn;
    }
}
