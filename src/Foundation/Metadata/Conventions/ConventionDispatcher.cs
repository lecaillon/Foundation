using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public class ConventionDispatcher
    {
        private readonly ConventionSet _conventionSet;

        public ConventionDispatcher(ConventionSet conventionSet)
        {
            Check.NotNull(conventionSet, nameof(conventionSet));

            _conventionSet = conventionSet;
        }

        public Entity OnEntityAdded(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            foreach (var entityTypeConvention in _conventionSet.EntityAddedConventions)
            {
                entity = entityTypeConvention.Apply(entity);
                if (entity == null)
                {
                    break;
                }
            }

            return entity;
        }

        public Entity OnBaseEntitySet(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            foreach (var entityTypeConvention in _conventionSet.BaseEntitySetConventions)
            {
                if (!entityTypeConvention.Apply(entity))
                {
                    break;
                }
            }

            return entity;
        }

        public Property OnPropertyAdded(Property property)
        {
            Check.NotNull(property, nameof(property));

            foreach (var propertyConvention in _conventionSet.PropertyAddedConventions)
            {
                property = propertyConvention.Apply(property);
                if (property == null)
                {
                    break;
                }
            }

            return property;
        }

        public Key OnPrimaryKeySet(Key key)
        {
            Check.NotNull(key, nameof(key));

            foreach (var keyConvention in _conventionSet.PrimaryKeySetConventions)
            {
                if (!keyConvention.Apply(key))
                {
                    break;
                }
            }

            return key;
        }

        public Key OnKeyAdded(Key key)
        {
            Check.NotNull(key, nameof(key));

            foreach (var keyConvention in _conventionSet.KeyAddedConventions)
            {
                key = keyConvention.Apply(key);
                if (key == null)
                {
                    break;
                }
            }

            return key;
        }
    }
}
