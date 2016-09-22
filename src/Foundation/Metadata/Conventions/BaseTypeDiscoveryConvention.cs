using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public class BaseTypeDiscoveryConvention : InheritanceDiscoveryConventionBase
    {
        public override Entity Apply(Entity entity)
        {
            if (entity.IsShadowEntity) return entity;

            var clrType = entity.ClrType;
            Check.NotNull(clrType, nameof(entity.ClrType));

            var baseEntity = FindClosestBaseType(entity);
            return baseEntity != null ? entity.TrySetBaseType(baseEntity)
                                      : entity;
        }
    }
}
