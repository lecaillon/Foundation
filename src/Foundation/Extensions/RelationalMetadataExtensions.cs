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
    }
}
