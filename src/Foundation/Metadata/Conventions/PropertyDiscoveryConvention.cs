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
                var primitiveProperties = entity.ClrType.GetRuntimeProperties().Where(IsCandidatePrimitiveProperty);
                foreach (var property in primitiveProperties)
                {
                    entity.GetOrAddProperty(property);
                }
            }

            return entity;
        }

        protected virtual bool IsCandidatePrimitiveProperty(PropertyInfo property)
        {
            Check.NotNull(property, nameof(property));

            return property.IsCandidateProperty()
                && property.PropertyType.IsPrimitive()
                && !property.IsInherited(); // ne doit pas héritée d'une classe abstraite par ex
        }
    }
}
