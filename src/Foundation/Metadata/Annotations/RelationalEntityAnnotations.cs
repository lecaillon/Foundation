using System;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalEntityAnnotations : IRelationalEntityAnnotations
    {
        private Entity _entity;

        public RelationalEntityAnnotations(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            _entity = entity;
        }

        protected virtual Entity Entity => _entity;

        protected virtual Annotable Metadata => Entity.Root();

        public virtual string TableName
        {
            get { return (string)Metadata[RelationalAnnotationNames.TableName] ?? Entity.Name; }
            set { Metadata[RelationalAnnotationNames.TableName] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public string Schema
        {
            get { throw new NotImplementedException(); /* return (string)Metadata[RelationalAnnotationNames.Schema] ?? GetAnnotations(Entity.Model).DefaultSchema; */ }
            set { Metadata[RelationalAnnotationNames.Schema] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        public Property DiscriminatorProperty
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public object DiscriminatorValue
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
