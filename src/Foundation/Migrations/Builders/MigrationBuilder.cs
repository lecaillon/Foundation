using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation.Metadata;
using Foundation.Metadata.Annotations;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

//IRelationalTypeMapper -> RelationalTypeMapper -> SqlServerTypeMapper
//IRelationalConnection -> RelationalConnection -> SqlServerConnection(IDbContextOptions -> DbContextOptions -> 

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

        public virtual OperationBuilder<AddColumnOperation> AddColumn(string name, string table, Type clrType, string type = null, bool? unicode = null, int? maxLength = null, bool rowVersion = false, string schema = null, bool nullable = false, object defaultValue = null, string defaultValueSql = null, string computedColumnSql = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new AddColumnOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                ClrType = clrType,
                ColumnType = type,
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                IsNullable = nullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql
            };
            Operations.Add(operation);

            return new OperationBuilder<AddColumnOperation>(operation);
        }

        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey(string name, string table, string column, string principalTable, string schema = null, string principalSchema = null, string principalColumn = null, ReferentialAction onUpdate = ReferentialAction.NoAction, ReferentialAction onDelete = ReferentialAction.NoAction)
            => AddForeignKey(
                name,
                table,
                new[] { column },
                principalTable,
                schema,
                principalSchema,
                new[] { principalColumn },
                onUpdate,
                onDelete);

        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey(string name, string table, string[] columns, string principalTable, string schema = null, string principalSchema = null, string[] principalColumns = null, ReferentialAction onUpdate = ReferentialAction.NoAction, ReferentialAction onDelete = ReferentialAction.NoAction)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(columns, nameof(columns));
            Check.NotEmpty(principalTable, nameof(principalTable));

            var operation = new AddForeignKeyOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                Columns = columns,
                PrincipalSchema = principalSchema,
                PrincipalTable = principalTable,
                PrincipalColumns = principalColumns,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            Operations.Add(operation);

            return new OperationBuilder<AddForeignKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey(string name, string table, string column, string schema = null)
            => AddPrimaryKey(
                name,
                table,
                new[] { column },
                schema);

        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey(string name, string table, string[] columns, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(columns, nameof(columns));

            var operation = new AddPrimaryKeyOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                Columns = columns
            };
            Operations.Add(operation);

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddUniqueConstraintOperation> AddUniqueConstraint(string name, string table, string column, string schema = null)
            => AddUniqueConstraint(
                name,
                table,
                new[] { column },
                schema);

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

        public virtual AlterOperationBuilder<AlterColumnOperation> AlterColumn(
            string name, 
            string table, 
            Type clrType,
            string type = null, 
            bool? unicode = null, 
            int? maxLength = null,
            bool rowVersion = false,
            string schema = null,
            bool nullable = false,
            object defaultValue = null,
            string defaultValueSql = null,
            string computedColumnSql = null,
            Type oldClrType = null,
            string oldType = null,
            bool? oldUnicode = null,
            int? oldMaxLength = null,
            bool oldRowVersion = false,
            bool oldNullable = false,
            object oldDefaultValue = null,
            string oldDefaultValueSql = null,
            string oldComputedColumnSql = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new AlterColumnOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                ClrType = clrType,
                ColumnType = type,
                IsUnicode = unicode,
                MaxLength = maxLength,
                IsRowVersion = rowVersion,
                IsNullable = nullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql,
                OldColumn = new ColumnOperation
                {
                    ClrType = oldClrType ?? clrType,
                    ColumnType = oldType,
                    IsUnicode = oldUnicode,
                    MaxLength = oldMaxLength,
                    IsRowVersion = oldRowVersion,
                    IsNullable = oldNullable,
                    DefaultValue = oldDefaultValue,
                    DefaultValueSql = oldDefaultValueSql,
                    ComputedColumnSql = oldComputedColumnSql
                }
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

        public virtual OperationBuilder<CreateIndexOperation> CreateIndex(string name, string table, string column, string schema = null, bool unique = false)
            => CreateIndex(
                name,
                table,
                new[] { column },
                schema,
                unique);

        public virtual OperationBuilder<CreateIndexOperation> CreateIndex(string name, string table, string[] columns, string schema = null, bool unique = false)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(columns, nameof(columns));

            var operation = new CreateIndexOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                Columns = columns,
                IsUnique = unique
            };
            Operations.Add(operation);

            return new OperationBuilder<CreateIndexOperation>(operation);
        }

        public virtual OperationBuilder<EnsureSchemaOperation> EnsureSchema(string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new EnsureSchemaOperation
            {
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<EnsureSchemaOperation>(operation);
        }

        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence(string name, string schema = null, long startValue = 1L, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false)
            => CreateSequence(name, typeof(long), schema, startValue, incrementBy, minValue, maxValue, cyclic);

        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence(string name, Type clrType, string schema = null, long startValue = 1L, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new CreateSequenceOperation
            {
                Schema = schema,
                Name = name,
                ClrType = clrType,
                StartValue = startValue,
                IncrementBy = incrementBy,
                MinValue = minValue,
                MaxValue = maxValue,
                IsCyclic = cyclic
            };
            Operations.Add(operation);

            return new OperationBuilder<CreateSequenceOperation>(operation);
        }

        public virtual CreateTableBuilder<TColumns> CreateTable<TColumns>(string name, Func<ColumnsBuilder, TColumns> columns, string schema = null, Action<CreateTableBuilder<TColumns>> constraints = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(columns, nameof(columns));

            var createTableOperation = new CreateTableOperation
            {
                Schema = schema,
                Name = name
            };

            var columnsBuilder = new ColumnsBuilder(createTableOperation);
            var columnsObject = columns(columnsBuilder);
            var columnMap = new Dictionary<PropertyInfo, AddColumnOperation>();
            foreach (var property in typeof(TColumns).GetTypeInfo().DeclaredProperties)
            {
                var addColumnOperation = (AddColumnOperation)property.GetMethod.Invoke(columnsObject, null);
                if (addColumnOperation.Name == null)
                {
                    addColumnOperation.Name = property.Name;
                }
                columnMap.Add(property, addColumnOperation);
            }

            var builder = new CreateTableBuilder<TColumns>(createTableOperation, columnMap);
            constraints?.Invoke(builder);

            Operations.Add(createTableOperation);

            return builder;
        }

        public virtual OperationBuilder<DropColumnOperation> DropColumn(string name, string table, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new DropColumnOperation
            {
                Schema = schema,
                Table = table,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropColumnOperation>(operation);
        }

        public virtual OperationBuilder<DropForeignKeyOperation> DropForeignKey(string name, string table, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new DropForeignKeyOperation
            {
                Schema = schema,
                Table = table,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropForeignKeyOperation>(operation);
        }

        public virtual OperationBuilder<DropIndexOperation> DropIndex(string name, string table = null, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropIndexOperation
            {
                Schema = schema,
                Table = table,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropIndexOperation>(operation);
        }

        public virtual OperationBuilder<DropPrimaryKeyOperation> DropPrimaryKey(string name, string table, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new DropPrimaryKeyOperation
            {
                Schema = schema,
                Table = table,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropPrimaryKeyOperation>(operation);
        }

        public virtual OperationBuilder<DropSchemaOperation> DropSchema(string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropSchemaOperation
            {
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropSchemaOperation>(operation);
        }

        public virtual OperationBuilder<DropSequenceOperation> DropSequence(string name, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropSequenceOperation
            {
                Schema = schema,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropSequenceOperation>(operation);
        }

        public virtual OperationBuilder<DropTableOperation> DropTable(string name, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropTableOperation
            {
                Schema = schema,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropTableOperation>(operation);
        }

        public virtual OperationBuilder<DropUniqueConstraintOperation> DropUniqueConstraint(string name, string table, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));

            var operation = new DropUniqueConstraintOperation
            {
                Schema = schema,
                Table = table,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropUniqueConstraintOperation>(operation);
        }

        public virtual OperationBuilder<RenameColumnOperation> RenameColumn(string name, string table, string newName, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(table, nameof(table));
            Check.NotEmpty(newName, nameof(newName));

            var operation = new RenameColumnOperation
            {
                Name = name,
                Schema = schema,
                Table = table,
                NewName = newName
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameColumnOperation>(operation);
        }

        public virtual OperationBuilder<RenameIndexOperation> RenameIndex(string name, string newName, string table = null, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));

            var operation = new RenameIndexOperation
            {
                Schema = schema,
                Table = table,
                Name = name,
                NewName = newName
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameIndexOperation>(operation);
        }

        public virtual OperationBuilder<RenameSequenceOperation> RenameSequence(string name, string schema = null, string newName = null, string newSchema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RenameSequenceOperation
            {
                Name = name,
                Schema = schema,
                NewName = newName,
                NewSchema = newSchema
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameSequenceOperation>(operation);
        }

        public virtual OperationBuilder<RenameTableOperation> RenameTable(string name, string schema = null, string newName = null, string newSchema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RenameTableOperation
            {
                Schema = schema,
                Name = name,
                NewName = newName,
                NewSchema = newSchema
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameTableOperation>(operation);
        }

        public virtual OperationBuilder<RestartSequenceOperation> RestartSequence(string name, long startValue = 1L, string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RestartSequenceOperation
            {
                Name = name,
                Schema = schema,
                StartValue = startValue
            };
            Operations.Add(operation);

            return new OperationBuilder<RestartSequenceOperation>(operation);
        }

        public virtual OperationBuilder<SqlOperation> Sql(string sql, bool suppressTransaction = false)
        {
            Check.NotEmpty(sql, nameof(sql));

            var operation = new SqlOperation
            {
                Sql = sql,
                SuppressTransaction = suppressTransaction
            };
            Operations.Add(operation);

            return new OperationBuilder<SqlOperation>(operation);
        }
    }
}
