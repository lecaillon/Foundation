using System.Collections.Generic;

namespace Foundation.Metadata.Conventions
{
    /// <summary>
    ///     Base implementation for a set of conventions used to build a model. This base implementation is an empty set of conventions.
    /// </summary>
    public class ConventionSet
    {
        /// <summary>
        ///     Conventions to run when an entity type is added to the model.
        /// </summary>
        public virtual IList<IEntityConvention> EntityAddedConventions { get; } = new List<IEntityConvention>();

        /// <summary>
        ///     Conventions to run when a base entity type is configured for an inheritance hierarchy.
        /// </summary>
        public virtual IList<IBaseTypeConvention> BaseEntitySetConventions { get; } = new List<IBaseTypeConvention>();

        /// <summary>
        ///     Conventions to run when a property is added.
        /// </summary>
        public virtual IList<IPropertyConvention> PropertyAddedConventions { get; } = new List<IPropertyConvention>();

        /// <summary>
        ///     Conventions to run when a primary key is configured.
        /// </summary>
        public virtual IList<IPrimaryKeyConvention> PrimaryKeySetConventions { get; } = new List<IPrimaryKeyConvention>();

        /// <summary>
        ///     Conventions to run when a key is added.
        /// </summary>
        public virtual IList<IKeyConvention> KeyAddedConventions { get; } = new List<IKeyConvention>();
    }
}
