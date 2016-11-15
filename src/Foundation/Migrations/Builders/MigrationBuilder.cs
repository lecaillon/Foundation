using System.Collections.Generic;
using Foundation.Metadata;
using Foundation.Metadata.Annotations;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class MigrationBuilder
    {
        public MigrationBuilder(IRelationalAnnotationProvider annotationProvider)
        {
            Check.NotNull(annotationProvider, nameof(annotationProvider));

            AnnotationProvider = annotationProvider;
        }

        public virtual IRelationalAnnotationProvider AnnotationProvider { get; }

        public virtual List<MigrationOperation> Operations { get; } = new List<MigrationOperation>();

        protected virtual CreateTableBuilder CreateTable(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            var pk = entity.FindDeclaredPrimaryKey();

            var entityAnnotaionProvider = AnnotationProvider.For(entity);
            var pkAnnotaionProvider = AnnotationProvider.For(pk);

            string schema = entityAnnotaionProvider.Schema;
            string name = entityAnnotaionProvider.TableName;

            var createTableOperation = new CreateTableOperation
            {
                Schema = schema,
                Name = name
            };

            var builder = new CreateTableBuilder(createTableOperation);
            builder.PrimaryKey(pkAnnotaionProvider.Name, entity.FindDeclaredPrimaryKey());

            Operations.Add(createTableOperation);

            return builder;
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
