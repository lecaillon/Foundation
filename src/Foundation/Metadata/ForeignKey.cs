using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents a relationship where a foreign key property(s) in a dependent entity type
    ///     reference a corresponding primary or alternate key in a principal entity type.
    /// </summary>
    public class ForeignKey
    {
        public ForeignKey(IReadOnlyList<Property> dependentProperties, Key principalKey, Entity dependentEntity, Entity principalEntity)
        {
            Check.NotEmpty(dependentProperties, nameof(dependentProperties));
            Check.HasNoNulls(dependentProperties, nameof(dependentProperties));
            Check.NotNull(principalKey, nameof(principalKey));
            Check.NotNull(principalEntity, nameof(principalEntity));

            Properties = dependentProperties;
            PrincipalKey = principalKey;
            DeclaringEntity = dependentEntity;
            PrincipalEntity = principalEntity;
        }

        /// <summary>
        ///     Gets the foreign key properties in the dependent entity.
        /// </summary>
        public IEnumerable<Property> Properties { get; }

        /// <summary>
        ///     Gets the dependent entity type. This may be different from the type that <see cref="Properties" />
        ///     are defined on when the relationship is defined a derived type in an inheritance hierarchy (since the properties
        ///     may be defined on a base type).
        /// </summary>
        public Entity DeclaringEntity { get; }

        /// <summary>
        ///     Gets the principal entity type that this relationship targets. This may be different from the type that
        ///     <see cref="PrincipalKey" /> is defined on when the relationship targets a derived type in an inheritance
        ///     hierarchy (since the key is defined on the base type of the hierarchy).
        /// </summary>
        public Entity PrincipalEntity { get; }

        /// <summary>
        ///     Gets the primary or alternate key that the relationship targets.
        /// </summary>
        public Key PrincipalKey { get; }

        /// <summary>
        ///     Gets the navigation property on the dependent entity type that points to the principal entity.
        /// </summary>
        public Navigation DependentToPrincipal { get; }

        /// <summary>
        ///     Gets the navigation property on the principal entity type that points to the dependent entity.
        /// </summary>
        public Navigation PrincipalToDependent { get; }

        /// <summary>
        ///     Gets a value indicating whether the values assigned to the foreign key properties are unique.
        /// </summary>
        public bool IsUnique { get; }

        /// <summary>
        ///     Gets a value indicating if this relationship is required. If true, the dependent entity must always be
        ///     assigned to a valid principal entity.
        /// </summary>
        public bool IsRequired { get; }

        /// <summary>
        ///     Gets a value indicating how a delete operation is applied to dependent entities in the relationship when the
        ///     principal is deleted or the relationship is severed.
        /// </summary>
        public DeleteBehavior DeleteBehavior { get; }

        public bool IsSelfReferencing => DeclaringEntity == PrincipalEntity;
    }
}