using System;
using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents a navigation property which can be used to navigate a relationship.
    /// </summary>
    public class Navigation : PropertyBase
    {
        public Navigation(PropertyInfo navigationProperty, ForeignKey fkToLinkedEntity, ForeignKey fkToTargetedEntity) : base(Check.NotNull(navigationProperty, nameof(navigationProperty)).Name, navigationProperty)
        {
            Check.NotNull(fkToLinkedEntity, nameof(fkToLinkedEntity));
            Check.NotNull(fkToTargetedEntity, nameof(fkToTargetedEntity));

            ForeignKeyToLinkedEntity = fkToLinkedEntity;
            ForeignKeyToTargetedEntity = fkToTargetedEntity;
        }

        /// <summary>
        ///     Gets the type that this property belongs to.
        /// </summary>
        public override Entity DeclaringEntity => ForeignKeyToLinkedEntity.PrincipalEntity;

        public override Type ClrType => PropertyInfo?.PropertyType ?? typeof(object);

        /// <summary>
        ///     Gets the foreign key used to navigate to the linked entity.
        /// </summary>
        public ForeignKey ForeignKeyToLinkedEntity { get; }

        /// <summary>
        ///     Gets the foreign key used to navigate from the linked entity to the targeted entity.
        /// </summary>
        public ForeignKey ForeignKeyToTargetedEntity { get; }

        /// <summary>
        ///     Gets the entity that this navigation targets.
        /// </summary>
        /// <returns> The target entity. </returns>
        public Entity GetTargetEntity() => ForeignKeyToTargetedEntity.PrincipalEntity;

        public override string ToString() => this.ToDebugString();
    }
}
