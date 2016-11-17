using Foundation.Metadata.Annotations;

namespace Foundation.Migrations.Operations
{
    public class AlterTableOperation : MigrationOperation, IAlterMigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual Annotable OldTable { get; set; } = new Annotable();
        Annotable IAlterMigrationOperation.OldAnnotations => OldTable;
    }
}
