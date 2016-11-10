using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation.Metadata;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class MigrationBuilder
    {
        public virtual List<MigrationOperation> Operations { get; } = new List<MigrationOperation>();

        protected virtual CreateTableBuilder CreateTable(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            string schema = string.Empty;
            string name = string.Empty;

            var createTableOperation = new CreateTableOperation
            {
                Schema = schema,
                Name = name
            };

            var builder = new CreateTableBuilder(createTableOperation);
            builder.PrimaryKey("", entity.FindDeclaredPrimaryKey());

            Operations.Add(createTableOperation);
            throw new NotImplementedException();
        }

        protected virtual OperationBuilder<AlterTableOperation> AlterTable(string name, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new AlterTableOperation
            {
                Schema = schema,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<AlterTableOperation>(operation);
        }
    }
}
