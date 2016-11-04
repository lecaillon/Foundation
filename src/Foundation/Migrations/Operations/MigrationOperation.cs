using Foundation.Metadata.Internal;

namespace Foundation.Migrations.Operations
{
    public abstract class MigrationOperation : Annotable
    {
        public virtual bool IsDestructiveChange { get; set; }
    }
}
