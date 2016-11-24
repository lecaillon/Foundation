using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Metadata.Annotations;
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

        private Property(string name, PropertyInfo propertyInfo, Type clrType, Entity declaringEntity) : base(name, propertyInfo)
        {
            Check.NotNull(declaringEntity, nameof(declaringEntity));
            Check.NotNull(clrType, nameof(clrType));

            DeclaringEntity = declaringEntity;
            ClrType = clrType;
        }

        /// <summary>
        ///     Gets the type that this property belongs to.
        /// </summary>
        public override Entity DeclaringEntity { get; }

        /// <summary>
        ///     Gets the type of value that this property holds.
        /// </summary>
        public override Type ClrType { get; }

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

        /// <summary>
        ///     Gets the maximum length of data that is allowed in this property. For example, if the property is a <see cref="string" /> '
        ///     then this is the maximum number of characters.
        /// </summary>
        /// <param name="property"> The property to get the maximum length of. </param>
        /// <returns> The maximum length, or null if none if defined. </returns>
        public int? MaxLength => (int?)this[CoreAnnotationNames.MaxLengthAnnotation];

        /// <summary>
        ///     Gets a value indicating whether or not the property can persist unicode characters.
        /// </summary>
        /// <param name="property"> The property to get the unicode setting for. </param>
        /// <returns> The unicode setting, or null if none if defined. </returns>
        public bool? IsUnicode =>  (bool?)this[CoreAnnotationNames.UnicodeAnnotation];

        public override string ToString() => this.ToDebugString();

        public static string Format(IEnumerable<Property> properties, bool includeTypes = false)
            => "{" + string.Join(", ", properties.Select(p => "'" + p.Name + "'" + (includeTypes ? " : " + p.ClrType.DisplayName(fullName: false) : ""))) + "}";

        /// <summary>
        ///     Gets all primary or alternate keys that use this property. Returns an empty list if none.
        /// </summary>
        public IEnumerable<Key> GetContainingKeys() => Keys ?? Enumerable.Empty<Key>();

        internal Property Clone(Entity targetEntity, string propertyName = null)
        {
            Check.NotNull(targetEntity, nameof(targetEntity));
            Check.NullButNotEmpty(propertyName, nameof(propertyName));

            return new Property(propertyName ?? Name, PropertyInfo, ClrType, targetEntity)
            {
                PrimaryKey = null,
                ForeignKeys  = null,
                Keys = null,
                Indexes = null
            };
        }
    }
}