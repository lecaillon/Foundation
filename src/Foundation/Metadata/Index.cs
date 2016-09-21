using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents an index on a set of properties.
    /// </summary>
    public class Index
    {
        public Index(IReadOnlyList<Property> properties, Entity declaringEntity)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotNull(declaringEntity, nameof(declaringEntity));

            Properties = properties;
            DeclaringEntity = declaringEntity;
        }

        /// <summary>
        ///     Gets the properties that this index is defined on.
        /// </summary>
        IEnumerable<Property> Properties { get; }

        /// <summary>
        ///     Gets the entity type the index is defined on. This may be different from the type that <see cref="Properties" />
        ///     are defined on when the index is defined a derived type in an inheritance hierarchy (since the properties
        ///     may be defined on a base type).
        /// </summary>
        public Entity DeclaringEntity { get; }

        /// <summary>
        ///     Gets a value indicating whether the values assigned to the indexed properties are unique.
        /// </summary>
        public bool IsUnique { get; }
    }
}