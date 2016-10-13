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

        public Entity OnEntityAddedStrict(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            foreach (var entityTypeConvention in _conventionSet.EntityAddedStrictConventions)
            {
                entity = entityTypeConvention.Apply(entity);
                if (entity == null)
                {
                    break;
                }
            }

            return entity;
        }

        public Entity OnEntityAdded(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            foreach (var entityTypeConvention in _conventionSet.EntityAddedFullConventions)
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

        public ForeignKey OnForeignKeyAdded(ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            foreach (var relationshipConvention in _conventionSet.ForeignKeyAddedConventions)
            {
                foreignKey = relationshipConvention.Apply(foreignKey);
                if (foreignKey == null)
                {
                    break;
                }
            }

            return foreignKey;
        }
    }
}
