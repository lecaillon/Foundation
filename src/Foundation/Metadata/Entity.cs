using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Metadata.Annotations;
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

        private readonly object _typeOrName;
        private readonly List<Entity> _directlyDerivedEntities = new List<Entity>();
        private readonly SortedDictionary<string, Property> _properties;
        private readonly SortedDictionary<IReadOnlyList<Property>, Key> _keys = new SortedDictionary<IReadOnlyList<Property>, Key>(PropertyListComparer.Instance);
        private readonly SortedDictionary<string, Navigation> _navigations = new SortedDictionary<string, Navigation>(StringComparer.Ordinal);
        private readonly SortedSet<ForeignKey> _foreignKeys = new SortedSet<ForeignKey>(ForeignKeyComparer.Instance);
        private Entity _baseType;
        private Key _primaryKey;

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name"> Name of the entity. </param>
        /// <param name="model"> Model this entity belongs to. </param>
        public Entity(string name, Model model) : this(model)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(model, nameof(model));

            _typeOrName = name;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="clrType"> Entity type. </param>
        /// <param name="model"> Model this entity belongs to. </param>
        public Entity(Type clrType, Model model) : this(model)
        {
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(model, nameof(model));

            _typeOrName = clrType;
        }

        private Entity(Model model)
        {
            Model = model;
            _properties = new SortedDictionary<string, Property>(new PropertyComparer(this));
        }

        #endregion

        /// <summary>
        ///     Gets the model this entity belongs to.
        /// </summary>
        public Model Model { get; }

        /// <summary>
        ///     <para>
        ///         Gets the CLR class that is used to represent instances of this entity. Returns null if the entity does not have a
        ///         corresponding CLR class (known as a shadow entity).
        ///     </para>
        /// </summary>
        public Type ClrType => _typeOrName as Type;

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
        public string Name => ClrType?.ShortDisplayName() ?? (string)_typeOrName;

        /// <summary>
        ///     Gets the base type of the entity. Returns null if this is not a derived type in an inheritance hierarchy.
        /// </summary>
        public Entity BaseType => _baseType;

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

        /// <summary>
        ///     Gets all the entities derived from this entity.
        /// </summary>
        public IEnumerable<Entity> GetDerivedEntities()
        {
            var derivedEntities = new List<Entity>();
            var entity = this;
            var index = 0;
            while (entity != null)
            {
                derivedEntities.AddRange(entity.GetDirectlyDerivedEntities());
                entity = derivedEntities.Count > index ? derivedEntities[index] : null;
                index++;
            }
            return derivedEntities;
        }

        #region Primary and Candidate Keys

        public IEnumerable<Key> GetKeys() => _baseType?.GetKeys().Concat(_keys.Values) ?? _keys.Values;

        public IEnumerable<Key> GetDeclaredKeys() => _keys.Values;

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
        public Key FindPrimaryKey() => _baseType?.FindPrimaryKey() ?? FindDeclaredPrimaryKey();

        public Key FindDeclaredPrimaryKey() => _primaryKey;

        internal Key SetPrimaryKey(Property property, bool runConventions = true) => SetPrimaryKey(property == null ? null : new[] { property });

        /// <summary>
        ///     Sets the primary key for this entity.
        /// </summary>
        /// <param name="properties"> The properties that make up the primary key. </param>
        /// <returns> The newly created key. </returns>
        internal Key SetPrimaryKey(IReadOnlyList<Property> properties)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));

            if (_baseType != null)
                throw new InvalidOperationException(ResX.DerivedEntityKey(Name, _baseType.Name));

            if (_primaryKey != null)
                throw new InvalidOperationException(ResX.PrimaryKeyAlreadyExists);

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

            return _primaryKey;
        }

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

        public IEnumerable<Property> FindPropertiesInHierarchy(string propertyName) => ToEnumerable(FindProperty(propertyName)).Concat(FindDerivedProperties(propertyName));

        public IEnumerable<Property> FindDerivedProperties(string propertyName)
        {
            Check.NotNull(propertyName, nameof(propertyName));

            return GetDerivedEntities().Select(et => et.FindDeclaredProperty(propertyName)).Where(p => p != null);
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

            return AddProperty(new Property(clrProperty, this));
        }

        internal Property AddProperty(Property property)
        {
            _properties.Add(property.Name, property);

            Model.ConventionDispatcher.OnPropertyAdded(property);

            return property;
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

        public Navigation FindNavigation(PropertyInfo propertyInfo) => FindNavigation(Check.NotNull(propertyInfo, nameof(propertyInfo)).Name);

        public Navigation FindNavigation(string name)
        {
            Check.NotEmpty(name, nameof(name));

            return FindDeclaredNavigation(name) ?? _baseType?.FindNavigation(name);
        }

        public Navigation FindDeclaredNavigation(string name)
        {
            Check.NotEmpty(name, nameof(name));

            Navigation navigation;
            return _navigations.TryGetValue(name, out navigation) ? navigation : null;
        }

        public IEnumerable<Navigation> FindNavigationsInHierarchy(string propertyName) 
            => ToEnumerable(FindNavigation(propertyName)).Concat(FindDerivedNavigations(propertyName));

        public IEnumerable<Navigation> FindDerivedNavigations(string navigationName)
        {
            Check.NotNull(navigationName, nameof(navigationName));

            return GetDerivedEntities().Select(et => et.FindDeclaredNavigation(navigationName)).Where(n => n != null);
        }

        public IEnumerable<Navigation> GetDeclaredNavigations() => _navigations.Values;

        public IEnumerable<Navigation> GetNavigations() => _baseType?.GetNavigations().Concat(_navigations.Values) ?? _navigations.Values;

        internal Navigation AddNavigation(PropertyInfo navigationProperty, ForeignKey fkToLinkedEntity, ForeignKey fkToTargetedEntity)
        {
            Check.NotNull(navigationProperty, nameof(navigationProperty));
            Check.NotNull(fkToLinkedEntity, nameof(fkToLinkedEntity));
            Check.NotNull(fkToTargetedEntity, nameof(fkToTargetedEntity));

            var name = navigationProperty.Name;
            var duplicateNavigation = FindNavigationsInHierarchy(name).FirstOrDefault();
            if (duplicateNavigation != null)
                throw new InvalidOperationException(ResX.DuplicateNavigation(name, Name, duplicateNavigation.DeclaringEntity.Name));

            var duplicateProperty = FindPropertiesInHierarchy(name).FirstOrDefault();
            if (duplicateProperty != null)
                throw new InvalidOperationException(ResX.ConflictingProperty(name, Name, duplicateProperty.DeclaringEntity.Name));

            var navigation = new Navigation(navigationProperty, fkToLinkedEntity, fkToTargetedEntity);

            _navigations.Add(name, navigation);

            return navigation;
        }

        internal void AddManyToManyRelationship(Entity targetEntity, PropertyInfo navigationProperty)
        {
            Check.NotNull(targetEntity, nameof(targetEntity));
            Check.NotNull(navigationProperty, nameof(navigationProperty));
            
            string associationTableName = navigationProperty.GetCustomAttribute<RelationshipAttribute>()?.AssociationTableName ??
                                          $"{Name}_{targetEntity.Name}";

            AddManyToManyRelationship(associationTableName, this, targetEntity, navigationProperty);
        }

        internal void AddManyToManyRelationship(Entity targetEntity, PropertyInfo navigationProperty, PropertyInfo inverseProperty)
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

            AddManyToManyRelationship(associationTableName, this, targetEntity, navigationProperty);

            var linkedEntity = Model.FindEntity(associationTableName);

            targetEntity.AddNavigation(inverseProperty,
                                       linkedEntity.GetForeignKeys().Single(x => x.PrincipalEntity == targetEntity),
                                       linkedEntity.GetForeignKeys().Single(x => x.PrincipalEntity == this));
        }

        private Navigation AddManyToManyRelationship(string associationTableName, Entity entity1, Entity entity2, PropertyInfo navigationProperty)
        {
            Check.NotEmpty(associationTableName, nameof(associationTableName));
            Check.NotNull(entity1, nameof(entity1));
            Check.NotNull(entity2, nameof(entity2));
            Check.NotNull(navigationProperty, nameof(navigationProperty));

            var linkedEntity = Model.AddLinkedEntity(associationTableName, entity1, entity2);

            return AddNavigation(navigationProperty,
                                 linkedEntity.GetForeignKeys().Single(x => x.PrincipalEntity == this),
                                 linkedEntity.GetForeignKeys().Single(x => x.PrincipalEntity == entity2));
        }

        #endregion

        #region Foreign Keys

        public ForeignKey FindForeignKey(Property property, Key principalKey, Entity principalEntity) => FindForeignKey(new[] { property }, principalKey, principalEntity);

        /// <summary>
        ///     Gets the foreign key for the given properties that points to a given primary or alternate key. Returns null
        ///     if no foreign key is found.
        /// </summary>
        /// <param name="properties"> The properties that the foreign key is defined on. </param>
        /// <param name="principalKey"> The primary or alternate key that is referenced. </param>
        /// <param name="principalEntity">
        ///     The entity that the relationship targets. This may be different from the type that <paramref name="principalKey" />
        ///     is defined on when the relationship targets a derived type in an inheritance hierarchy (since the key is defined on the
        ///     base type of the hierarchy).
        /// </param>
        /// <returns> The foreign key, or null if none is defined. </returns>
        public ForeignKey FindForeignKey(IReadOnlyList<Property> properties, Key principalKey, Entity principalEntity)
        {
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotEmpty(properties, nameof(properties));
            Check.NotNull(principalKey, nameof(principalKey));
            Check.NotNull(principalEntity, nameof(principalEntity));

            return FindDeclaredForeignKey(properties, principalKey, principalEntity) ?? _baseType?.FindForeignKey(properties, principalKey, principalEntity);
        }

        public ForeignKey FindDeclaredForeignKey(IReadOnlyList<Property> properties, Key principalKey, Entity principalEntity)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.NotNull(principalKey, nameof(principalKey));
            Check.NotNull(principalEntity, nameof(principalEntity));

            return FindDeclaredForeignKeys(properties).SingleOrDefault(fk => PropertyListComparer.Instance.Equals(fk.PrincipalKey.Properties, principalKey.Properties) &&
                                                                             StringComparer.Ordinal.Equals(fk.PrincipalEntity.Name, principalEntity.Name));
        }

        public IEnumerable<ForeignKey> FindDeclaredForeignKeys(IReadOnlyList<Property> properties)
        {
            Check.NotEmpty(properties, nameof(properties));

            return _foreignKeys.Where(fk => PropertyListComparer.Instance.Equals(fk.Properties, properties));
        }

        public IEnumerable<ForeignKey> FindForeignKeysInHierarchy(IReadOnlyList<Property> properties, Key principalKey, Entity principalEntity)
            => ToEnumerable(FindForeignKey(properties, principalKey, principalEntity)).Concat(FindDerivedForeignKeys(properties, principalKey, principalEntity));

        public IEnumerable<ForeignKey> FindDerivedForeignKeys(IReadOnlyList<Property> properties, Key principalKey, Entity principalEntity)
            => GetDerivedEntities().Select(et => et.FindDeclaredForeignKey(properties, principalKey, principalEntity)).Where(fk => fk != null);

        internal ForeignKey AddForeignKey(Property property, Key principalKey, Entity principalEntity) => AddForeignKey(new[] { property }, principalKey, principalEntity);

        public IEnumerable<ForeignKey> GetReferencingForeignKeys() => _baseType?.GetReferencingForeignKeys().Concat(GetDeclaredReferencingForeignKeys()) 
                                                                      ?? GetDeclaredReferencingForeignKeys();

        public IEnumerable<ForeignKey> GetDeclaredReferencingForeignKeys() => DeclaredReferencingForeignKeys ?? Enumerable.Empty<ForeignKey>();

        private List<ForeignKey> DeclaredReferencingForeignKeys { get; set; }

        /// <summary>
        ///     Gets the foreign keys defined on this entity.
        /// </summary>
        /// <returns> The foreign keys defined on this entity. </returns>
        public IEnumerable<ForeignKey> GetForeignKeys() => _baseType?.GetForeignKeys().Concat(_foreignKeys) ?? _foreignKeys;

        public IEnumerable<ForeignKey> GetDeclaredForeignKeys() => _foreignKeys;

        /// <summary>
        ///     Adds a new relationship to this entity.
        /// </summary>
        /// <param name="properties"> The properties that the foreign key is defined on. </param>
        /// <param name="principalKey"> The primary or alternate key that is referenced. </param>
        /// <param name="principalEntity">
        ///     The entity type that the relationship targets. This may be different from the type that <paramref name="principalKey" />
        ///     is defined on when the relationship targets a derived type in an inheritance hierarchy (since the key is defined on the
        ///     base type of the hierarchy).
        /// </param>
        /// <returns> The newly created foreign key. </returns>
        internal ForeignKey AddForeignKey(IReadOnlyList<Property> properties, Key principalKey, Entity principalEntity, bool runConventions = true)
        {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotNull(principalKey, nameof(principalKey));
            Check.NotNull(principalEntity, nameof(principalEntity));

            foreach (var property in properties)
            {
                var actualProperty = FindProperty(property.Name);
                if (actualProperty == null || !actualProperty.DeclaringEntity.IsAssignableFrom(property.DeclaringEntity))
                {
                    throw new InvalidOperationException(ResX.ForeignKeyPropertiesWrongEntity(Property.Format(properties), Name));
                }

                if (actualProperty.GetContainingKeys().Any(k => k.DeclaringEntity != this))
                {
                    throw new InvalidOperationException(ResX.ForeignKeyPropertyInKey(actualProperty.Name, Name));
                }
            }

            var duplicateForeignKey = FindForeignKeysInHierarchy(properties, principalKey, principalEntity).FirstOrDefault();
            if (duplicateForeignKey != null)
            {
                throw new InvalidOperationException(ResX.DuplicateForeignKey(Property.Format(properties), Name, duplicateForeignKey.DeclaringEntity.Name, Property.Format(principalKey.Properties), principalEntity.Name));
            }

            var foreignKey = new ForeignKey(properties, principalKey, this, principalEntity);

            _foreignKeys.Add(foreignKey);

            foreach (var property in properties)
            {
                var currentKeys = property.ForeignKeys;
                if (currentKeys == null)
                {
                    property.ForeignKeys = new ForeignKey[] { foreignKey };
                }
                else
                {
                    var newKeys = currentKeys.ToList();
                    newKeys.Add(foreignKey);
                    property.ForeignKeys = newKeys;
                }
            }

            if (principalKey.ReferencingForeignKeys == null)
            {
                principalKey.ReferencingForeignKeys = new List<ForeignKey> { foreignKey };
            }
            else
            {
                principalKey.ReferencingForeignKeys.Add(foreignKey);
            }

            if (principalEntity.DeclaredReferencingForeignKeys == null)
            {
                principalEntity.DeclaredReferencingForeignKeys = new List<ForeignKey> { foreignKey };
            }
            else
            {
                principalEntity.DeclaredReferencingForeignKeys.Add(foreignKey);
            }

            if (runConventions)
                Model.ConventionDispatcher.OnForeignKeyAdded(foreignKey);

            return foreignKey;
        }

        #endregion

        public override string ToString() => this.ToDebugString();

        private static IEnumerable<T> ToEnumerable<T>(T element) where T : class 
            => element == null ? Enumerable.Empty<T>() : new[] { element };
    }
}