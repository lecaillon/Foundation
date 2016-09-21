using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents a navigation property which can be used to navigate a relationship.
    ///     http://ef.readthedocs.io/en/latest/modeling/relationships.html
    /// </summary>
    public class Navigation : PropertyBase
    {
        public Navigation(PropertyInfo navigationProperty, ForeignKey foreignKey) : base(Check.NotNull(navigationProperty, nameof(navigationProperty)).Name, navigationProperty)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            ForeignKey = foreignKey;
        }

        /// <summary>
        ///     Gets the type that this property belongs to.
        /// </summary>
        public override Entity DeclaringEntity { get; }

        /// <summary>
        ///     Gets the foreign key that defines the relationship this navigation property will navigate.
        /// </summary>
        public ForeignKey ForeignKey { get; }

        /// <summary>
        ///     Gets a value indicating whether the given navigation property is the navigation property on the dependent entity
        ///     type that points to the principal entity.
        /// </summary>
        /// <returns>
        ///     True if the given navigation property is the navigation property on the dependent entity
        ///     type that points to the principal entity, otherwise false.
        /// </returns>
        public bool IsDependentToPrincipal => ForeignKey.DependentToPrincipal == this;

        /// <summary>
        ///     Gets the entity type that a given navigation property will hold an instance of
        ///     (or hold instances of if it is a collection navigation).
        /// </summary>
        /// <returns> The target entity type. </returns>
        public Entity GetTargetType()
        {
            return IsDependentToPrincipal ? ForeignKey.PrincipalEntity : ForeignKey.DeclaringEntity;
        }
    }
}
