using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class AlterOperationBuilder<TOperation> : OperationBuilder<TOperation> where TOperation : MigrationOperation, IAlterMigrationOperation
    {
        public AlterOperationBuilder(TOperation operation) : base(operation)
        {
        }

        public new virtual AlterOperationBuilder<TOperation> Annotation(string name, object value)
            => (AlterOperationBuilder<TOperation>)base.Annotation(name, value);

        public virtual AlterOperationBuilder<TOperation> OldAnnotation(string name, object value)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(value, nameof(value));

            Operation.OldAnnotations.AddAnnotation(name, value);

            return this;
        }
    }
}
