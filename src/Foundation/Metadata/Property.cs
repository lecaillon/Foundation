using System;
using System.Collections.Generic;
using System.Reflection;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents a scalar property of an entity.
    /// </summary>
    public class Property : PropertyBase
    {
        public Property(PropertyInfo propertyInfo, Entity declaringEntity) : base(Check.NotNull(propertyInfo, nameof(propertyInfo)).Name, propertyInfo)
        {
            Check.NotNull(declaringEntity, nameof(declaringEntity));

            DeclaringEntity = declaringEntity;
            ClrType = propertyInfo.PropertyType;
        }

        /// <summary>
        ///     Gets the type that this property belongs to.
        /// </summary>
        public override Entity DeclaringEntity { get; }

        /// <summary>
        ///     Gets the type of value that this property holds.
        /// </summary>
        public Type ClrType { get; }

        /// <summary>
        ///     Gets a value indicating whether this property can contain null.
        /// </summary>
        public bool IsNullable => ClrType.IsNullableType();

        /// <summary>
        ///     Gets a value indicating whether this is a shadow property. A shadow property is one that does not have a
        ///     corresponding property in the entity class.
        /// </summary>
        public bool IsShadowProperty => PropertyInfo == null;

        /// <summary>
        ///     Gets the primary key that uses this property (including a composite primary key in which this property
        ///     is included) or null if it is not part of the primary key.
        /// </summary>
        public Key PrimaryKey { get; set; }

        /// <summary>
        ///     Gets the foreign keys that use this property (including a composite FK in which this property
        ///     is included) or null if it is not part of a FK.
        /// </summary>
        public IReadOnlyList<ForeignKey> ForeignKeys { get; set; }

        /// <summary>
        ///     Gets the primary or alternate keys that use this property (or composite primary or alternate key).
        /// </summary>
        public IReadOnlyList<Key> Keys { get; set; }

        /// <summary>
        ///     Gets the indexes that use this property (or part of a composite index).
        /// </summary>
        public IReadOnlyList<Index> Indexes { get; set; }

        public override string ToString() => this.ToDebugString();
    }
}