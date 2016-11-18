namespace Foundation.Metadata.Annotations
{
    public interface IRelationalPropertyAnnotations
    {
        string ColumnName { get; set; }
        string ColumnType { get; set; }
        string DefaultValueSql { get; set; }
        string ComputedColumnSql { get; set; }
        object DefaultValue { get; set; }
        string OldColumnName { get; set; }
    }
}
