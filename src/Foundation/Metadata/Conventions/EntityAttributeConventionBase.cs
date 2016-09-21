using System;
using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata.Conventions
{
    public abstract class EntityAttributeConventionBase<TAttribute> : IEntityConvention where TAttribute : Attribute
    {
        public virtual Entity Apply(Entity entity)
        {
            Check.NotNull(entity, nameof(entity));

            var attributes = entity.ClrType?.GetTypeInfo().GetCustomAttributes<TAttribute>(true);
            if (attributes != null)
            {
                foreach (var attribute in attributes)
                {
                    entity = Apply(entity, attribute);
                    if (entity == null)
                    {
                        break;
                    }
                }
            }

            return entity;
        }

        public abstract Entity Apply(Entity entity, TAttribute attribute);
    }
}
