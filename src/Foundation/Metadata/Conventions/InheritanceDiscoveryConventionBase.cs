using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public abstract class InheritanceDiscoveryConventionBase : IEntityConvention
    {
        public abstract Entity Apply(Entity entity);

        protected virtual Entity FindClosestBaseType(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            if (entity.IsShadowEntity) return null;

            var clrType = entity.ClrType;
            Check.NotNull(clrType, nameof(entity.ClrType));

            var baseType = clrType.GetTypeInfo().BaseType;
            Entity baseEntity = null;

            while ((baseType != null && baseType != typeof(object)) && (baseEntity == null))
            {
                baseEntity = entity.Model.GetOrAddEntity(baseType);
                baseType = baseType.GetTypeInfo().BaseType;
            }

            return baseEntity;
        }
    }
}
