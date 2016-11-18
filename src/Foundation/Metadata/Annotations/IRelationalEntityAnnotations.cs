namespace Foundation.Metadata.Annotations
{
    public interface IRelationalEntityAnnotations
    {
        string TableName { get; set; }
        string OldTableName { get; set; }
        string Schema { get; set; }
        Property DiscriminatorProperty { get; set; }
        object DiscriminatorValue { get; set; }
    }
}
