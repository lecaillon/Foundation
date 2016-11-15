using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalEntityAnnotations : IRelationalEntityAnnotations
    {
        public RelationalEntityAnnotations(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            Entity = entity;
        }

        protected virtual Entity Entity { get; }

        protected virtual Annotable Metadata => Entity.Root();

        public virtual string TableName
        {
            get { return (string)Metadata[RelationalAnnotationNames.TableName] ?? Entity.Name; }
            set { Metadata[RelationalAnnotationNames.TableName] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public string Schema
        {
            get { return (string)Metadata[RelationalAnnotationNames.Schema] ?? new RelationalModelAnnotations(Entity.Model).DefaultSchema; }
            set { Metadata[RelationalAnnotationNames.Schema] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public Property DiscriminatorProperty { get; set; }

        public object DiscriminatorValue { get; set; }
    }
}
