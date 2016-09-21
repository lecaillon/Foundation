using System;
using System.Linq;
using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public class PropertyDiscoveryConvention : IEntityConvention
    {
        public Entity Apply(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (!entity.IsShadowEntity)
            {
                var primitiveProperties = entity.ClrType.GetRuntimeProperties().Where(prop => IsCandidatePrimitiveProperty(prop, entity.ClrType));
                foreach (var property in primitiveProperties)
                {
                    entity.GetOrAddProperty(property);
                }
            }

            return entity;
        }

        protected virtual bool IsCandidatePrimitiveProperty(PropertyInfo property, Type entityType)
        {
            Check.NotNull(property, nameof(property));

            return property.IsCandidateProperty()
                && property.PropertyType.IsPrimitive()
                && property.DeclaringType == entityType; // ne doit pas héritée d'une classe abstraite par ex
        }
    }
}
