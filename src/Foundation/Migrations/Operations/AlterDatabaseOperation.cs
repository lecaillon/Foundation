using Foundation.Metadata.Annotations;

namespace Foundation.Migrations.Operations
{
    public class AlterDatabaseOperation : MigrationOperation, IAlterMigrationOperation
    {
        public virtual Annotable OldDatabase { get; } = new Annotable();
        Annotable IAlterMigrationOperation.OldAnnotations => OldDatabase;
    }
}
