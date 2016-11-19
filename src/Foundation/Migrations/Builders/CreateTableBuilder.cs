using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class CreateTableBuilder<TColumns> : OperationBuilder<CreateTableOperation>
    {
        private readonly IReadOnlyDictionary<PropertyInfo, AddColumnOperation> _columnMap;

        public CreateTableBuilder(CreateTableOperation operation, IReadOnlyDictionary<PropertyInfo, AddColumnOperation> columnMap) : base(operation)
        {
            Check.NotNull(columnMap, nameof(columnMap));

            _columnMap = columnMap;
        }

        public virtual OperationBuilder<AddForeignKeyOperation> ForeignKey(
              string name,
              Expression<Func<TColumns, object>> column,
              string principalTable,
              string principalColumn,
              string principalSchema = null,
              ReferentialAction onUpdate = ReferentialAction.NoAction,
              ReferentialAction onDelete = ReferentialAction.NoAction)
              => ForeignKey(
                  name,
                  column,
                  principalTable,
                  new[] { principalColumn },
                  principalSchema,
                  onUpdate,
                  onDelete);

        public virtual OperationBuilder<AddForeignKeyOperation> ForeignKey(
            string name,
            Expression<Func<TColumns, object>> columns,
            string principalTable,
            string[] principalColumns,
            string principalSchema = null,
            ReferentialAction onUpdate = ReferentialAction.NoAction,
            ReferentialAction onDelete = ReferentialAction.NoAction)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(columns, nameof(columns));
            Check.NotEmpty(principalTable, nameof(principalTable));

            var operation = new AddForeignKeyOperation
            {
                Schema = Operation.Schema,
                Table = Operation.Name,
                Name = name,
                Columns = Map(columns),
                PrincipalSchema = principalSchema,
                PrincipalTable = principalTable,
                PrincipalColumns = principalColumns,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            Operation.ForeignKeys.Add(operation);

            return new OperationBuilder<AddForeignKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddPrimaryKeyOperation> PrimaryKey(string name, Expression<Func<TColumns, object>> columns)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(columns, nameof(columns));

            var operation = new AddPrimaryKeyOperation
            {
                Schema = Operation.Schema,
                Table = Operation.Name,
                Name = name,
                Columns = Map(columns)
            };
            Operation.PrimaryKey = operation;

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
        }

        public virtual OperationBuilder<AddUniqueConstraintOperation> UniqueConstraint(string name, Expression<Func<TColumns, object>> columns)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(columns, nameof(columns));

            var operation = new AddUniqueConstraintOperation
            {
                Schema = Operation.Schema,
                Table = Operation.Name,
                Name = name,
                Columns = Map(columns)
            };
            Operation.UniqueConstraints.Add(operation);

            return new OperationBuilder<AddUniqueConstraintOperation>(operation);
        }

        public new virtual CreateTableBuilder<TColumns> Annotation(string name, object value) => (CreateTableBuilder<TColumns>)base.Annotation(name, value);

        private string[] Map(LambdaExpression columns) => columns.GetPropertyAccessList().Select(c => _columnMap[c].Name).ToArray();
    }
}
