namespace Foundation.Migrations.Operations
{
    public class AddForeignKeyOperation : MigrationOperation
    {
        public virtual string Name { get; set; }
        public virtual string Schema { get; set; }
        public virtual string Table { get; set; }
        public virtual string[] Columns { get; set; }
        public virtual string PrincipalSchema { get; set; }
        public virtual string PrincipalTable { get; set; }
        public virtual string[] PrincipalColumns { get; set; }
        public virtual ReferentialAction OnUpdate { get; set; }
        public virtual ReferentialAction OnDelete { get; set; }
    }
}
