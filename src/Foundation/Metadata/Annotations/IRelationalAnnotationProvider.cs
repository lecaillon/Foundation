namespace Foundation.Metadata.Annotations
{
    public interface IRelationalAnnotationProvider
    {
        IRelationalModelAnnotations For(Model model);
        IRelationalEntityAnnotations For(Entity entity);
        IRelationalForeignKeyAnnotations For(ForeignKey foreignKey);
        IRelationalIndexAnnotations For(Index index);
        IRelationalKeyAnnotations For(Key key);
        IRelationalPropertyAnnotations For(Property property);
    }
}
