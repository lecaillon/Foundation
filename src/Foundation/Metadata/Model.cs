using System;
using System.Collections.Generic;
using Foundation.Metadata.Conventions;
using Foundation.Utilities;

namespace Foundation.Metadata
{
    /// <summary>
    ///     Metadata about the shape of entities, the relationships between them, and how they map to the database.
    /// </summary>
    public class Model
    {
        private readonly SortedDictionary<string, Entity> _entities = new SortedDictionary<string, Entity>();
        private readonly IDictionary<Type, Entity> _clrTypeMap = new Dictionary<Type, Entity>();

        public Model() : this(new ConventionSet())
        {
        }

        public Model(ConventionSet conventions)
        {
            ConventionDispatcher = new ConventionDispatcher(conventions);
            //ConventionDispatcher.OnModelInitialized(Builder);
        }

        public ConventionDispatcher ConventionDispatcher { get; }

        /// <summary>
        ///     Gets all entity types defined in the model.
        /// </summary>
        public IEnumerable<Entity> GetEntities() => _entities.Values;

#if DEBUG
        public Entity GetOrAddEntityForDebugMode(Type type) => GetOrAddEntity(type);
#endif

        public Entity FindEntity(Type type)
        {
            Check.NotNull(type, nameof(type));

            Entity entity;
            return _clrTypeMap.TryGetValue(type, out entity) ? entity : FindEntity(type.DisplayName());
        }

        public Entity FindEntity<T>()
        {
            return FindEntity(typeof(T));
        }

        public virtual Entity FindEntity(string name)
        {
            Check.NotEmpty(name, nameof(name));

            Entity entity;
            return _entities.TryGetValue(name, out entity) ? entity : null;
        }

        /// <summary>
        ///     Attempts to add the specified entity to this model.
        /// </summary>
        /// <returns> The entity. </returns>
        internal Entity GetOrAddEntity(Type type)
        {
            Check.NotNull(type, nameof(type));

            var entity = FindEntity(type);
            if (entity == null)
                return AddEntity(type, runConventions: true);

            return entity;
        }

        /// <summary>
        ///     Attempts to add the specified entity to this model.
        /// </summary>
        /// <returns> The entity. </returns>
        internal virtual Entity GetOrAddEntity(string name) => FindEntity(name) ?? AddEntity(name);

        internal Entity AddEntity(Type type, bool runConventions = true)
        {
            Check.NotNull(type, nameof(type));

            var entity = new Entity(type, this);
            _clrTypeMap[type] = entity;

            return AddEntity(entity, runConventions);
        }

        internal Entity AddEntity(string name, bool runConventions = true)
        {
            Check.NotEmpty(name, nameof(name));

            var entity = new Entity(name, this);

            return AddEntity(entity, runConventions);
        }

        private Entity AddEntity(Entity entity, bool runConventions = true)
        {
            _entities[entity.Name] = entity;

            if (runConventions)
                ConventionDispatcher.OnEntityAdded(entity);

            return entity;
        }

        public override string ToString() => this.ToDebugString();

        private class TypeNameComparer : IComparer<Type>
        {
            public static readonly TypeNameComparer Instance = new TypeNameComparer();

            private TypeNameComparer()
            {
            }

            public int Compare(Type x, Type y) => StringComparer.Ordinal.Compare(x.Name, y.Name);
        }
    }
}