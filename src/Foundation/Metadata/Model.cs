using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        ///     Gets the entity with the given name. Returns null if no entity type with the given name is found.
        /// </summary>
        /// <param name="name"> The name of the entity type to find. </param>
        /// <returns> The entity type, or null if none are found. </returns>
        public Entity FindEntity(string name)
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
        internal Entity GetOrAddEntity(string name) => FindEntity(name) ?? AddEntity(name);

        /// <summary>
        ///     Adds an entity to the model.
        /// </summary>
        /// <param name="clrType"> The CLR class that is used to represent instances of this entity type. </param>
        /// <returns> The new entity. </returns>
        internal Entity AddEntity(Type type, bool runConventions = true)
        {
            Check.NotNull(type, nameof(type));

            var entity = new Entity(type, this);
            _clrTypeMap[type] = entity;

            return AddEntity(entity, runConventions);
        }

        /// <summary>
        ///     Adds a shadow state entity type to the model.
        /// </summary>
        /// <param name="name"> The name of the entity to be added. </param>
        /// <returns> The new entity type. </returns>
        internal Entity AddEntity(string name, bool runConventions = true)
        {
            Check.NotEmpty(name, nameof(name));

            var entity = new Entity(name, this);

            return AddEntity(entity, runConventions);
        }

        /// <summary>
        ///     <para>
        ///         Add a shadow entity to this model used to represent a many to many association between two entities.
        ///     </para>
        ///     <para>
        ///         Add PK
        ///         Add FK targeting <paramref name="entity1"/> 
        ///         Add FK targeting <paramref name="entity2"/> .
        ///     </para>
        /// </summary>
        /// <param name="name"> The name of the entity to be added. </param>
        /// <param name="entity1"> Declaring entity </param>
        /// <param name="entity2"> Targeting entity </param>
        /// <returns> The new entity. </returns>
        internal Entity AddLinkedEntity(string name, Entity entity1, Entity entity2)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(entity1, nameof(entity1));
            Check.NotNull(entity2, nameof(entity2));

            if (FindEntity(name) != null)
                throw new InvalidOperationException(ResX.DuplicateEntity(name));

            var linkedEntity = AddEntity(name, runConventions: false);

            var properties = new List<Property>();
            entity1.FindPrimaryKey().Properties.Concat(entity2.FindPrimaryKey().Properties)
                                    .ToList()
                                    .ForEach(x =>
                                    {
                                        string propertyName = x.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                                            ? "Id" + x.DeclaringEntity.Name
                                            : x.Name;
                                                
                                        linkedEntity.AddProperty(x.Clone(linkedEntity, propertyName));
                                    });

            linkedEntity.SetPrimaryKey(linkedEntity.GetDeclaredProperties().ToList());
            linkedEntity.AddForeignKey(linkedEntity.GetDeclaredProperties().Where(x => x.PropertyInfo.DeclaringType == entity1.ClrType).ToList(), entity1.FindPrimaryKey(), entity1);
            linkedEntity.AddForeignKey(linkedEntity.GetDeclaredProperties().Where(x => x.PropertyInfo.DeclaringType == entity2.ClrType).ToList(), entity2.FindPrimaryKey(), entity2);

            return linkedEntity;
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