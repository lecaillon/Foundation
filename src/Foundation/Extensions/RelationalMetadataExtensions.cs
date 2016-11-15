using Foundation.Metadata.Annotations;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Relational database specific extension methods for metadata.
    /// </summary>
    public static class RelationalMetadataExtensions
    {
        /// <summary>
        ///     Gets the relational database specific metadata for a model.
        /// </summary>
        /// <param name="model"> The model to get metadata for. </param>
        /// <returns> The relational database specific metadata for the model. </returns>
        public static RelationalModelAnnotations Relational(this Model model)
            => new RelationalModelAnnotations(Check.NotNull(model, nameof(model)));

        /// <summary>
        ///     Gets the relational database specific metadata for an entity.
        /// </summary>
        /// <param name="entity"> The entity to get metadata for. </param>
        /// <returns> The relational database specific metadata for the entity. </returns>
        public static IRelationalEntityAnnotations Relational(this Entity entity)
            => new RelationalEntityAnnotations(Check.NotNull(entity, nameof(entity)));

        /// <summary>
        ///     Gets the relational database specific metadata for a key.
        /// </summary>
        /// <param name="key"> The key to get metadata for. </param>
        /// <returns> The relational database specific metadata for the key. </returns>
        public static IRelationalKeyAnnotations Relational(this Key key)
            => new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)));

        /// <summary>
        ///     Gets the relational database specific metadata for a property.
        /// </summary>
        /// <param name="property"> The property to get metadata for. </param>
        /// <returns> The relational database specific metadata for the property. </returns>
        public static IRelationalPropertyAnnotations Relational(this Property property)
            => new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)));

        /// <summary>
        ///     Gets the relational database specific metadata for an index.
        /// </summary>
        /// <param name="index"> The index to get metadata for. </param>
        /// <returns> The relational database specific metadata for the index. </returns>
        public static IRelationalIndexAnnotations Relational(this Index index)
            => new RelationalIndexAnnotations(Check.NotNull(index, nameof(index)));

        /// <summary>
        ///     Gets the relational database specific metadata for a foreign key.
        /// </summary>
        /// <param name="foreignKey"> The foreign key to get metadata for. </param>
        /// <returns> The relational database specific metadata for the foreign key. </returns>
        public static IRelationalForeignKeyAnnotations Relational(this ForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)));
    }
}
