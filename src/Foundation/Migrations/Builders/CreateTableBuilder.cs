using System;
using Foundation.Metadata;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class CreateTableBuilder : OperationBuilder<CreateTableOperation>
    {
        public CreateTableBuilder(CreateTableOperation operation) : base(operation)
        {

        }

        public virtual OperationBuilder<AddPrimaryKeyOperation> PrimaryKey(string name, Key pk)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(pk, nameof(pk));

            if (pk.IsPrimaryKey == false)
                throw new InvalidOperationException(ResX.KeyNotPrimary(Property.Format(pk.Properties)));

            var operation = new AddPrimaryKeyOperation
            {
                Schema = Operation.Schema,
                Table = Operation.Name,
                Name = name,
                Columns = null
            };
            Operation.PrimaryKey = operation;

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
        }
    }
}
