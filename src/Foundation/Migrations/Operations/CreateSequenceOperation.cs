using System;

namespace Foundation.Migrations.Operations
{
    public class CreateSequenceOperation : SequenceOperation
    {
        public virtual string Schema { get; set; }
        public virtual string Name { get; set; }
        public virtual Type ClrType { get; set; }
        public virtual long StartValue { get; set; } = 1L;
    }
}
