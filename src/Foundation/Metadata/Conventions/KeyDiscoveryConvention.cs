using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public class KeyDiscoveryConvention : IEntityConvention, IPropertyConvention
    {
        private const string KeySuffix = "Id";

        public Entity Apply(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (entity.BaseType == null && entity.FindPrimaryKey() == null) // TODO : virer la condition FindPrimaryKey()
            {
                var candidateProperties = entity.GetProperties().Where(p => !p.IsShadowProperty).ToList();
                var keyProperty = DiscoverKeyProperties(entity, candidateProperties);
                if(keyProperty != null)
                {
                    entity.SetPrimaryKey(keyProperty);
                }
            }

            return entity;
        }

        public Property Apply(Property property)
        {
            Check.NotNull(property, nameof(property));

            Apply(property.DeclaringEntity);

            return property;
        }

        private Property DiscoverKeyProperties(Entity entity, IReadOnlyList<Property> candidateProperties)
        {
            Check.NotNull(entity, nameof(entity));

            // Id
            var keyProperty = candidateProperties.SingleOrDefault(p => string.Equals(p.Name, KeySuffix, StringComparison.OrdinalIgnoreCase));

            // PersonId
            if (keyProperty == null)
                keyProperty = candidateProperties.SingleOrDefault(p => string.Equals(p.Name, entity.Name + KeySuffix, StringComparison.OrdinalIgnoreCase));

            // IdPerson
            if (keyProperty == null)
                keyProperty = candidateProperties.SingleOrDefault(p => string.Equals(p.Name, KeySuffix + entity.Name, StringComparison.OrdinalIgnoreCase));

            return keyProperty;
        }
    }
}
