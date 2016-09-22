using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Metadata.DataAnnotations;
using Foundation.Metadata.Internal;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Represents an entity in an <see cref="Model" />.
    /// </summary>
    public class Entity : Annotable
    {
        #region Fields

        private readonly List<Entity> _directlyDerivedEntities = new List<Entity>();
        private readonly SortedDictionary<string, Property> _properties;
        private readonly SortedDictionary<IReadOnlyList<Property>, Key> _keys = new SortedDictionary<IReadOnlyList<Property>, Key>(PropertyListComparer.Instance);
        private readonly SortedDictionary<string, Navigation> _navigations = new SortedDictionary<string, Navigation>(StringComparer.Ordinal);
        private Entity _baseType;
        private Key _primaryKey;

        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="clrType"> Entity type. </param>
        /// <param name="model"> Model this entity belongs to. </param>
        public Entity(Type clrType, Model model)
        {
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(model, nameof(model));

            ClrType = clrType;
            Model = model;
            _properties = new SortedDictionary<string, Property>(new PropertyComparer(this));
        }

        /// <summary>
        ///     Gets the model this entity belongs to.
        /// </summary>
        public Model Model { get; }

        /// <summary>
        ///     <para>
        ///         Gets the CLR class that is used to represent instances of this entity. Returns null if the entity does not have a
        ///         corresponding CLR class (known as a shadow entity).
        ///     </para>
        ///     <para>
        ///         Shadow entities are not currently supported in a model that is used at runtime with a <see cref="DbContext" />.
        ///         Therefore, shadow entities will only exist in migration model snapshots, etc.
        ///     </para>
        /// </summary>
        public Type ClrType { get; }

        /// <summary>
        ///     Returns true if the entity is not linked to a real type.
        ///     Appens for entities used to reprensent many to many relationship.
        /// </summary>
        public bool IsShadowEntity => ClrType == null;

        /// <summary>
        ///     Gets a value indicating whether the Type is abstract.
        /// </summary>
        public bool IsAbstract => ClrType?.GetTypeInfo().IsAbstract ?? false;

        /// <summary>
        ///     Gets the name of the entity.
        /// </summary>
        public string Name => ClrType.ShortDisplayName();

        /// <summary>
        ///     Gets the base type of the entity. Returns null if this is not a derived type in an inheritance hierarchy.
        /// </summary>
        public Entity BaseType => _baseType;

        /// <summary>
        ///     Gets the foreign keys defined on this entity.
        /// </summary>
        public IReadOnlyList<ForeignKey> ForeignKeys { get; }

        /// <summary>
        ///     Gets the indexes defined on this entity.
        /// </summary>
        public IReadOnlyList<Index> Indexes { get; }

        /// <summary>
        ///     Attempts to add the specified base entity to this entity.
        /// </summary>
        /// <returns> This entity. </returns>
        internal Entity TrySetBaseType(Entity baseEntity)
        {
            Check.NotNull(baseEntity, nameof(baseEntity));

            if (_baseType != null && _baseType != baseEntity)
                throw new InvalidOperationException(ResX.EntityBaseTypeAlreadyDefined);

            if (_baseType == null)
            {
                _baseType = baseEntity;
                _baseType._directlyDerivedEntities.Add(this);
                Model.ConventionDispatcher.OnBaseEntitySet(this);
            }

            return this;
        }

        /// <summary>
        ///     Gets the entities directly derived from this entity.
        /// </summary>
        public IReadOnlyList<Entity> GetDirectlyDerivedEntities() => _directlyDerivedEntities;

        #region Primary and Candidate Keys

        internal Key GetOrAddKey(IReadOnlyList<Property> properties) => FindKey(properties) ?? AddKey(properties);

        /// <summary>
        ///     Gets the primary or alternate key that is defined on the given properties. 
        ///     Returns null if no key is defined for the given properties.
        /// </summary>
        /// <param name="properties"> The properties that make up the key. </param>
        /// <returns> The key, or null if none is defined. </returns>
        public Key FindKey(IReadOnlyList<Property> properties)
        {
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotEmpty(properties, nameof(properties));

            return FindDeclaredKey(properties) ?? _baseType?.FindKey(properties);
        }

        public Key FindDeclaredKey(IReadOnlyList<Property> properties)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));

            Key key;
            return _keys.TryGetValue(properties, out key) ? key : null;
        }

        /// <summary>
        ///     Adds a new alternate key to this entity type.
        /// </summary>
        /// <param name="properties"> The properties that make up the alternate key. </param>
        /// <returns> The newly created key. </returns>
        internal Key AddKey(IReadOnlyList<Property> properties)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));

            if (_baseType != null)
                throw new InvalidOperationException(ResX.DerivedEntityKey(Name, _baseType.Name));

            foreach (var property in properties)
            {
                if (FindProperty(property.Name) != property)
                    throw new InvalidOperationException(ResX.KeyPropertiesWrongEntity(Property.Format(properties), Name));

                if (property.ForeignKeys != null && property.ForeignKeys.Any(k => k.DeclaringEntity != this))
                    throw new InvalidOperationException(ResX.KeyPropertyInForeignKey(property.Name, Name));

                if (property.IsNullable)
                    throw new InvalidOperationException(ResX.NullableKey(Name, property.Name));
            }

            var key = FindKey(properties);
            if (key != null)
                throw new InvalidOperationException(ResX.DuplicateKey(Property.Format(properties), Name, key.DeclaringEntity.Name));

            key = new Key(properties);
            _keys.Add(properties, key);

            foreach (var property in properties)
            {
                var currentKeys = property.Keys;
                if (currentKeys == null)
                {
                    property.Keys = new Key[] { key };
                }
                else
                {
                    var newKeys = currentKeys.ToList();
                    newKeys.Add(key);
                    property.Keys = newKeys;
                }
            }

            return Model.ConventionDispatcher.OnKeyAdded(key);
        }

        /// <summary>
        ///     <para>
        ///         Gets primary key for this entity. Returns null if no primary key is defined.
        ///     </para>
        ///     <para>
        ///         To be a valid model, each entity type must have a primary key defined. />.
        ///     </para>
        /// </summary>
        /// <returns> The primary key, or null if none is defined. </returns>
        public virtual Key FindPrimaryKey() => _baseType?.FindPrimaryKey() ?? FindDeclaredPrimaryKey();

        public virtual Key FindDeclaredPrimaryKey() => _primaryKey;

        /// <summary>
        ///     Sets the primary key for this entity.
        /// </summary>
        /// <param name="properties"> The properties that make up the primary key. </param>
        /// <returns> The newly created key. </returns>
        internal virtual Key SetPrimaryKey(IReadOnlyList<Property> properties)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));

            if (_baseType != null)
                throw new InvalidOperationException(ResX.DerivedEntityKey(Name, _baseType.Name));

            Key key = GetOrAddKey(properties);
            foreach (var property in key.Properties)
            {
                _properties.Remove(property.Name);
                property.PrimaryKey = key;
            }

            _primaryKey = key;

            foreach (var property in key.Properties)
            {
                _properties.Add(property.Name, property);
            }

            Model.ConventionDispatcher.OnPrimaryKeySet(_primaryKey);

            return _primaryKey;
        }

        internal virtual Key SetPrimaryKey(Property property) => SetPrimaryKey(property == null ? null : new[] { property });

        #endregion

        #region Property

        /// <summary>
        ///     <para>
        ///         Gets only the properties defined on this entity, not those inherited from the BaseType.
        ///     </para>
        ///     <para>
        ///         This API only returns scalar properties and does not return navigation properties.
        ///     </para>
        /// </summary>
        public IEnumerable<Property> GetDeclaredProperties() => _properties.Values;

        /// <summary>
        ///     <para>
        ///         Gets all the properties defined on this entity included inherited properties.
        ///     </para>
        ///     <para>
        ///         This API only returns scalar properties and does not return navigation properties.
        ///     </para>
        /// </summary>
        /// <returns> The properties defined on this entity. </returns>
        public IEnumerable<Property> GetProperties() => _baseType?.GetProperties().Concat(_properties.Values) ?? _properties.Values;

        /// <summary>
        ///     <para>
        ///         Gets the property with a given name. Returns null if no property with the given name is defined.
        ///     </para>
        ///     <para>
        ///         This API only finds scalar properties and does not find navigation properties. Use
        ///         <see cref="EntityTypeExtensions.FindNavigation(IEntityType, string)" /> to find a navigation property.
        ///     </para>
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <returns> The property, or null if none is found. </returns>
        public Property FindProperty(string name) => FindDeclaredProperty(Check.NotEmpty(name, nameof(name))) ?? _baseType?.FindProperty(name);

        public Property FindProperty(PropertyInfo propertyInfo) => FindProperty(propertyInfo.Name);

        public Property FindDeclaredProperty(string propertyName)
        {
            Check.NotEmpty(propertyName, nameof(propertyName));

            Property property;
            return _properties.TryGetValue(propertyName, out property) ? property : null;
        }

        /// <summary>
        ///     Attempts to add the specified property to this entity.
        /// </summary>
        /// <returns> True if the property was added to the model successfully; False if the property already exists. </returns>
        internal Property GetOrAddProperty(PropertyInfo clrProperty) => GetOrAddProperty(clrProperty.Name, clrProperty.PropertyType, clrProperty);

        private Property GetOrAddProperty(string propertyName, Type propertyType, PropertyInfo clrProperty)
        {
            Check.NotEmpty(propertyName, nameof(propertyName));
            Check.NotNull(propertyType, nameof(propertyType));
            Check.NotNull(clrProperty, nameof(clrProperty));

            // IsIgnored -> false
            var existingProperty = FindProperty(propertyName);
            if (existingProperty != null)
                return existingProperty;

            var property = new Property(clrProperty, this);
            _properties.Add(propertyName, property);
            return Model.ConventionDispatcher.OnPropertyAdded(property);
        }

        private class PropertyComparer : IComparer<string>
        {
            private readonly Entity _entity;

            public PropertyComparer(Entity entity)
            {
                _entity = entity;
            }

            public int Compare(string x, string y)
            {
                var properties = _entity.FindPrimaryKey()?.Properties.Select(p => p.Name).ToList();

                var xIndex = -1;
                var yIndex = -1;

                if (properties != null)
                {
                    xIndex = properties.IndexOf(x);
                    yIndex = properties.IndexOf(y);
                }

                // Neither property is part of the Primary Key
                // Compare the property names
                if ((xIndex == -1) && (yIndex == -1))
                {
                    return StringComparer.Ordinal.Compare(x, y);
                }

                // Both properties are part of the Primary Key
                // Compare the indices
                if ((xIndex > -1) && (yIndex > -1))
                {
                    return xIndex - yIndex;
                }

                // One property is part of the Primary Key
                // The primary key property is first
                return xIndex > yIndex ? -1 : 1;
            }
        }

        #endregion

        #region Navigation

        public virtual Navigation FindNavigation(PropertyInfo propertyInfo) => FindNavigation(Check.NotNull(propertyInfo, nameof(propertyInfo)).Name);

        public virtual Navigation FindNavigation(string name)
        {
            Check.NotEmpty(name, nameof(name));

            return FindDeclaredNavigation(name) ?? _baseType?.FindNavigation(name);
        }

        public virtual Navigation FindDeclaredNavigation(string name)
        {
            Check.NotEmpty(name, nameof(name));

            Navigation navigation;
            return _navigations.TryGetValue(name, out navigation) ? navigation : null;
        }

        public virtual IEnumerable<Navigation> GetDeclaredNavigations() => _navigations.Values;

        public virtual IEnumerable<Navigation> GetNavigations() => _baseType?.GetNavigations().Concat(_navigations.Values) ?? _navigations.Values;

        internal Tuple<Navigation, Navigation> AddRelationship(Entity targetEntity, PropertyInfo navigationProperty, PropertyInfo inverseProperty)
        {
            Check.NotNull(targetEntity, nameof(targetEntity));
            Check.NotNull(navigationProperty, nameof(navigationProperty));
            Check.NotNull(inverseProperty, nameof(inverseProperty));

            string associationTableName;
            string navAssociationTableName = navigationProperty.GetCustomAttribute<RelationshipAttribute>()?.AssociationTableName;
            string inverseAssociationTableName = inverseProperty.GetCustomAttribute<RelationshipAttribute>()?.AssociationTableName;

            if (!string.IsNullOrWhiteSpace(navAssociationTableName))
            {
                if (!string.IsNullOrWhiteSpace(inverseAssociationTableName) &&
                    !navAssociationTableName.Equals(inverseAssociationTableName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException(ResX.IncoherentAssociationTableNames);
                }

                associationTableName = navAssociationTableName;
            }
            else
            {
                associationTableName = string.IsNullOrWhiteSpace(inverseAssociationTableName)
                    ? $"{Name}_{targetEntity.Name}"
                    : inverseAssociationTableName;
            }

            if (Model.FindEntity(associationTableName) != null)
                throw new InvalidOperationException(ResX.DuplicateEntity(associationTableName));

            var navigation = AddNavigation(targetEntity, navigationProperty, associationTableName);






            throw new NotImplementedException();
        }

        internal Navigation AddNavigation(Entity targetEntity, PropertyInfo navigationProperty)
        {
            Check.NotNull(targetEntity, nameof(targetEntity));
            Check.NotNull(navigationProperty, nameof(navigationProperty));

            string associationTableName = navigationProperty.GetCustomAttribute<RelationshipAttribute>()?.AssociationTableName ??
                                          $"{Name}_{targetEntity.Name}";

            if (Model.FindEntity(associationTableName) != null)
                throw new InvalidOperationException(ResX.DuplicateEntity(associationTableName));

            return AddNavigation(targetEntity, navigationProperty, associationTableName);
        }

        private Navigation AddNavigation(Entity targetEntity, PropertyInfo navigation, string associationTableName)
        {
            Check.NotNull(targetEntity, nameof(targetEntity));
            Check.NotNull(navigation, nameof(navigation));
            Check.NotEmpty(associationTableName, nameof(associationTableName));

            if (Model.FindEntity(associationTableName) != null)
                throw new InvalidOperationException(ResX.DuplicateEntity(associationTableName));

            // create new shadow entity

            throw new NotImplementedException();
        }

        #endregion

        public override string ToString() => this.ToDebugString();
    }
}