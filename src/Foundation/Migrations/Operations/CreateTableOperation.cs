using System.Collections.Generic;

namespace Foundation.Migrations.Operations
{
    public class CreateTableOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual AddPrimaryKeyOperation PrimaryKey { get; set; }
        public virtual List<AddColumnOperation> Columns { get; } = new List<AddColumnOperation>();
        public virtual List<AddForeignKeyOperation> ForeignKeys { get; } = new List<AddForeignKeyOperation>();
        public virtual List<AddUniqueConstraintOperation> UniqueConstraints { get; } = new List<AddUniqueConstraintOperation>();
    }
}
