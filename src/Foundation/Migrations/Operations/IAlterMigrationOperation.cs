using Foundation.Metadata.Annotations;

namespace Foundation.Migrations.Operations
{
    public interface IAlterMigrationOperation
    {
        Annotable OldAnnotations { get; }
    }
}
