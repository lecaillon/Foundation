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
        ///     Gets the relational database specific metadata for an entity.
        /// </summary>
        /// <param name="entity"> The entity to get metadata for. </param>
        /// <returns> The relational database specific metadata for the entity. </returns>
        public static IRelationalEntityAnnotations Relational(this Entity entity)
            => new RelationalEntityAnnotations(Check.NotNull(entity, nameof(entity)));
    }
}
