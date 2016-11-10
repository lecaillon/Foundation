// <auto-generated />
namespace Foundation
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    ///		This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static partial class ResX
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Foundation.Properties.ResX", typeof(ResX).GetTypeInfo().Assembly);

        /// <summary>
        /// The string argument '{argumentName}' cannot be empty.
        /// </summary>
        public static string ArgumentIsEmpty(object argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ArgumentIsEmpty", "argumentName"), argumentName);
        }

        /// <summary>
        /// The property '{property}' of the argument '{argument}' cannot be null.
        /// </summary>
        public static string ArgumentPropertyNull(object property, object argument)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ArgumentPropertyNull", "property", "argument"), property, argument);
        }

        /// <summary>
        /// The collection argument '{argumentName}' must contain at least one element.
        /// </summary>
        public static string CollectionArgumentIsEmpty(object argumentName)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("CollectionArgumentIsEmpty", "argumentName"), argumentName);
        }

        /// <summary>
        /// The navigation property '{navigation}' cannot be added to the entity '{entity}' because a property with the same name already exists on entity '{duplicateEntity}'.
        /// </summary>
        public static string ConflictingProperty(object navigation, object entity, object duplicateEntity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ConflictingProperty", "navigation", "entity", "duplicateEntity"), navigation, entity, duplicateEntity);
        }

        /// <summary>
        /// A key cannot be configured on '{derivedType}' because it is a derived type. The key must be configured on the root type '{rootType}'.
        /// </summary>
        public static string DerivedEntityKey(object derivedType, object rootType)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DerivedEntityKey", "derivedType", "rootType"), derivedType, rootType);
        }

        /// <summary>
        /// The annotation '{annotation}' cannot be added because an annotation with the same name already exists.
        /// </summary>
        public static string DuplicateAnnotation(object annotation)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DuplicateAnnotation", "annotation"), annotation);
        }

        /// <summary>
        /// The entity '{entity}' cannot be added to the model because an entity with the same name already exists.
        /// </summary>
        public static string DuplicateEntity(object entity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DuplicateEntity", "entity"), entity);
        }

        /// <summary>
        /// The foreign key {foreignKey} cannot be added to the entity '{entity}' because a foreign key on the same properties already exists on entity '{duplicateEntity}' and also targets the key {key} on '{principal}'.
        /// </summary>
        public static string DuplicateForeignKey(object foreignKey, object entity, object duplicateEntity, object key, object principal)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DuplicateForeignKey", "foreignKey", "entity", "duplicateEntity", "key", "principal"), foreignKey, entity, duplicateEntity, key, principal);
        }

        /// <summary>
        /// The key {key} cannot be added to the entity type '{entityType}' because a key on the same properties already exists on entity type '{duplicateEntityType}'.
        /// </summary>
        public static string DuplicateKey(object key, object entityType, object duplicateEntityType)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DuplicateKey", "key", "entityType", "duplicateEntityType"), key, entityType, duplicateEntityType);
        }

        /// <summary>
        /// The navigation property '{navigation}' cannot be added to the entity '{entity}' because a navigation property with the same name already exists on entity '{duplicateEntity}'.
        /// </summary>
        public static string DuplicateNavigation(object navigation, object entity, object duplicateEntity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("DuplicateNavigation", "navigation", "entity", "duplicateEntity"), navigation, entity, duplicateEntity);
        }

        /// <summary>
        /// The base entity cannot be added to the entity because a base entity is already defined.
        /// </summary>
        public static string EntityBaseTypeAlreadyDefined
        {
            get { return GetString("EntityBaseTypeAlreadyDefined"); }
        }

        /// <summary>
        /// Une clé primaire existe déjà.
        /// </summary>
        public static string EntityKeyAlreadyExists
        {
            get { return GetString("EntityKeyAlreadyExists"); }
        }

        /// <summary>
        /// The number of properties specified for the foreign key {foreignKey} on entity '{dependent}' does not match the number of properties in the principal key {principalKey} on entity '{principal}'.
        /// </summary>
        public static string ForeignKeyCountMismatch(object foreignKey, object dependent, object principalKey, object principal)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ForeignKeyCountMismatch", "foreignKey", "dependent", "principalKey", "principal"), foreignKey, dependent, principalKey, principal);
        }

        /// <summary>
        /// The specified foreign key properties {foreignKey} are not declared on the entity '{entity}'. Ensure foreign key properties are declared on the target entity.
        /// </summary>
        public static string ForeignKeyPropertiesWrongEntity(object foreignKey, object entity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ForeignKeyPropertiesWrongEntity", "foreignKey", "entity"), foreignKey, entity);
        }

        /// <summary>
        /// The property '{property}' cannot be part of a foreign key on '{entity}' because it is contained in a key defined on a base entity.
        /// </summary>
        public static string ForeignKeyPropertyInKey(object property, object entity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ForeignKeyPropertyInKey", "property", "entity"), property, entity);
        }

        /// <summary>
        /// The provided principal entity key '{principalKey}' is not a key on the entity '{principalEntity}'.
        /// </summary>
        public static string ForeignKeyReferencedEntityKeyMismatch(object principalKey, object principalEntity)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ForeignKeyReferencedEntityKeyMismatch", "principalKey", "principalEntity"), principalKey, principalEntity);
        }

        /// <summary>
        /// The types of the properties specified for the foreign key {foreignKey} on entity '{dependent}' do not match the types of the properties in the principal key {principalKey} on entity '{principal}'.
        /// </summary>
        public static string ForeignKeyTypeMismatch(object foreignKey, object dependent, object principalKey, object principal)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("ForeignKeyTypeMismatch", "foreignKey", "dependent", "principalKey", "principal"), foreignKey, dependent, principalKey, principal);
        }

        /// <summary>
        /// Navigation and inverse properties must define the same association table name.
        /// </summary>
        public static string IncoherentAssociationTableNames
        {
            get { return GetString("IncoherentAssociationTableNames"); }
        }

        /// <summary>
        /// The specified key {key} must be primary.
        /// </summary>
        public static string KeyNotPrimary(object key)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("KeyNotPrimary", "key"), key);
        }

        /// <summary>
        /// The specified key properties {key} are not declared on the entity type '{entityType}'. Ensure key properties are declared on the target entity type.
        /// </summary>
        public static string KeyPropertiesWrongEntity(object key, object entityType)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("KeyPropertiesWrongEntity", "key", "entityType"), key, entityType);
        }

        /// <summary>
        /// The property '{property}' cannot be part of a key on '{entityType}' because it is contained in a foreign key defined on a derived entity type.
        /// </summary>
        public static string KeyPropertyInForeignKey(object property, object entityType)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("KeyPropertyInForeignKey", "property", "entityType"), property, entityType);
        }

        /// <summary>
        /// A key on entity type '{entityType}' cannot contain the property '{property}' because it is nullable/optional. All properties on which a key is declared must be marked as non-nullable/required.
        /// </summary>
        public static string NullableKey(object entityType, object property)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("NullableKey", "entityType", "property"), entityType, property);
        }

        /// <summary>
        /// A temporary primary key cannot be added because a primary key is already defined.
        /// </summary>
        public static string PrimaryKeyAlreadyExists
        {
            get { return GetString("PrimaryKeyAlreadyExists"); }
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
