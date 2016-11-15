using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalModelAnnotations : IRelationalModelAnnotations
    {
        public RelationalModelAnnotations(Model model)
        {
            Check.NotNull(model, nameof(model));

            Model = model;
        }

        protected virtual Model Model { get; }

        protected virtual Annotable Metadata => Model;

        public string DatabaseName
        {
            get { return (string)Metadata[RelationalAnnotationNames.DatabaseName]; }
            set { Metadata[RelationalAnnotationNames.DatabaseName] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public string DefaultSchema
        {
            get { return (string)Metadata[RelationalAnnotationNames.DefaultSchema]; }
            set { Metadata[RelationalAnnotationNames.DefaultSchema] = Check.NullButNotEmpty(value, nameof(value)); }
        }
    }
}
