using System;

namespace Foundation.Migrations.Operations
{
    public class ColumnOperation : MigrationOperation
    {
        public virtual Type ClrType { get; set; }
        public virtual string ColumnType { get; set; }
        public virtual bool? IsUnicode { get; set; }
        public virtual int? MaxLength { get; set; }
        public virtual bool IsRowVersion { get; set; }
        public virtual bool IsNullable { get; set; }
        public virtual object DefaultValue { get; set; }
        public virtual string DefaultValueSql { get; set; }
        public virtual string ComputedColumnSql { get; set; }
    }
}
