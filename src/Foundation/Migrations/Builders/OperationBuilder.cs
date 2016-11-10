using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class OperationBuilder<TOperation> where TOperation : MigrationOperation
    {
        public OperationBuilder(TOperation operation)
        {
            Check.NotNull(operation, nameof(operation));

            Operation = operation;
        }

        public virtual TOperation Operation { get; }

        public virtual OperationBuilder<TOperation> Annotation(string name, object value)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(value, nameof(value));

            Operation.AddAnnotation(name, value);

            return this;
        }
    }
}
