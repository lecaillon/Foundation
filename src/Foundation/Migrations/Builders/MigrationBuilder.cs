using System;
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

        public virtual OperationBuilder<AddColumnOperation> AddColumn(Property property)
        {
            Check.NotNull(property, nameof(property));

            var entityAnnotationProvider = AnnotationProvider.For(property.DeclaringEntity.Root());
            var propertyAnnotationProvider = AnnotationProvider.For(property);

            var operation = new AddColumnOperation
            {
                Schema = entityAnnotationProvider.Schema,
                Table = entityAnnotationProvider.TableName,
                Name = propertyAnnotationProvider.ColumnName,
                ClrType = property.ClrType,
                ColumnType = propertyAnnotationProvider.ColumnType,
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                IsNullable = property.IsNullable,
                DefaultValue = propertyAnnotationProvider.DefaultValue,
                DefaultValueSql = propertyAnnotationProvider.DefaultValueSql,
                ComputedColumnSql = propertyAnnotationProvider.ComputedColumnSql
            };
            Operations.Add(operation);

            return new OperationBuilder<AddColumnOperation>(operation);
        }

        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey(ForeignKey fk)
        {
            Check.NotNull(fk, nameof(fk));

            var entityAnnotationProvider = AnnotationProvider.For(fk.DeclaringEntity.Root());
            var principalAnnotationProvider = AnnotationProvider.For(fk.PrincipalEntity.Root());
            var fkAnnotationProvider = AnnotationProvider.For(fk);

            var operation = new AddForeignKeyOperation
            {
                Schema = entityAnnotationProvider.Schema,
                Table = entityAnnotationProvider.TableName,
                Name = fkAnnotationProvider.Name,
                Columns = columns,
                PrincipalSchema = principalAnnotationProvider.Schema,
                PrincipalTable = principalAnnotationProvider.TableName,
                PrincipalColumns = principalColumns,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            Operations.Add(operation);

            return new OperationBuilder<AddForeignKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey(Key pk)
        {
            Check.NotNull(pk, nameof(pk));

            var entityAnnotationProvider = AnnotationProvider.For(pk.DeclaringEntity.Root());
            var pkAnnotationProvider = AnnotationProvider.For(pk);

            var operation = new AddPrimaryKeyOperation
            {
                Schema = entityAnnotationProvider.Schema,
                Table = entityAnnotationProvider.TableName,
                Name = pkAnnotationProvider.Name,
                Columns = columns
            };
            Operations.Add(operation);

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddUniqueConstraintOperation> AddUniqueConstraint(string name, string table, string[] columns, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(columns, nameof(columns));

            var operation = new AddUniqueConstraintOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                Columns = columns
            };
            Operations.Add(operation);

            return new OperationBuilder<AddUniqueConstraintOperation>(operation);
        }

        public virtual AlterOperationBuilder<AlterColumnOperation> AlterColumn(Property property, ColumnOperation oldColumn)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(oldColumn, nameof(oldColumn));

            var entityAnnotationProvider = AnnotationProvider.For(property.DeclaringEntity.Root());
            var propertyAnnotationProvider = AnnotationProvider.For(property);

            var operation = new AlterColumnOperation
            {
                Schema = entityAnnotationProvider.Schema,
                Table = entityAnnotationProvider.TableName,
                Name = propertyAnnotationProvider.ColumnName,
                ClrType = property.ClrType,
                ColumnType = propertyAnnotationProvider.ColumnType,
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                IsNullable = property.IsNullable,
                DefaultValue = propertyAnnotationProvider.DefaultValue,
                DefaultValueSql = propertyAnnotationProvider.DefaultValueSql,
                ComputedColumnSql = propertyAnnotationProvider.ComputedColumnSql,
                OldColumn = oldColumn
            };

            Operations.Add(operation);

            return new AlterOperationBuilder<AlterColumnOperation>(operation);
        }

        public virtual AlterOperationBuilder<AlterDatabaseOperation> AlterDatabase()
        {
            var operation = new AlterDatabaseOperation();
            Operations.Add(operation);

            return new AlterOperationBuilder<AlterDatabaseOperation>(operation);
        }

        public virtual AlterOperationBuilder<AlterSequenceOperation> AlterSequence(string name, string schema = null, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false, int oldIncrementBy = 1, long? oldMinValue = null, long? oldMaxValue = null, bool oldCyclic = false)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new AlterSequenceOperation
            {
                Schema = schema,
                Name = name,
                IncrementBy = incrementBy,
                MinValue = minValue,
                MaxValue = maxValue,
                IsCyclic = cyclic,
                OldSequence = new SequenceOperation
                {
                    IncrementBy = oldIncrementBy,
                    MinValue = oldMinValue,
                    MaxValue = oldMaxValue,
                    IsCyclic = oldCyclic
                }
            };
            Operations.Add(operation);

            return new AlterOperationBuilder<AlterSequenceOperation>(operation);
        }






        public virtual AlterOperationBuilder<AlterTableOperation> AlterTable(string name, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new AlterTableOperation
            {
                Schema = schema,
                Name = name
            };
            Operations.Add(operation);

            return new AlterOperationBuilder<AlterTableOperation>(operation);
        }







        protected virtual CreateTableBuilder CreateTable(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            var pk = entity.FindDeclaredPrimaryKey();

            var entityAnnotationProvider = AnnotationProvider.For(entity);
            var pkAnnotationProvider = AnnotationProvider.For(pk);

            string schema = entityAnnotationProvider.Schema;
            string name = entityAnnotationProvider.TableName;

            var createTableOperation = new CreateTableOperation
            {
                Schema = schema,
                Name = name
            };

            var builder = new CreateTableBuilder(createTableOperation);
            builder.PrimaryKey(pkAnnotationProvider.Name, entity.FindDeclaredPrimaryKey());

            Operations.Add(createTableOperation);

            return builder;
        }

        protected virtual OperationBuilder<AlterTableOperation> AlterTable(AlterTableOperation table, )
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
