using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Metadata.Annotations;

namespace Foundation.Metadata.Conventions
{
    public class RelationshipDiscoveryConvention : IEntityConvention
    {
        public const string NAVIGATION_CANDIDATES_ANNOTATION_NAME = "RelationshipDiscoveryConvention:NavigationCandidates";
        public const string AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME = "RelationshipDiscoveryConvention:AmbiguousNavigations";

        public Entity Apply(Entity entity)
        {
            if (entity.IsShadowEntity) return entity;

            if (entity.FindAnnotation(NAVIGATION_CANDIDATES_ANNOTATION_NAME) == null)
            {
                var model = entity.Model;
                var discoveredEntities = new List<Entity>();
                var unvisitedEntities = new Stack<Entity>();
                unvisitedEntities.Push(entity);

                while (unvisitedEntities.Count > 0)
                {
                    var nextEntity = unvisitedEntities.Pop();
                    discoveredEntities.Add(nextEntity);
                    var navigationCandidates = GetNavigationCandidates(nextEntity).Reverse();
                    foreach (var candidateTuple in navigationCandidates)
                    {
                        var targetClrType = candidateTuple.Value;
                        if (model.FindEntity(targetClrType) != null /* || nextEntity.IsIgnored(candidateTuple.Key.Name, ConfigurationSource.Convention) */ )
                        {
                            continue;
                        }

                        var candidateTargetEntity = model.AddEntity(targetClrType, runConventions: false);
                        if (candidateTargetEntity != null)
                        {
                            unvisitedEntities.Push(candidateTargetEntity);
                        }
                    }
                }

                for (var i = 1; i < discoveredEntities.Count; i++)
                {
                    model.ConventionDispatcher.OnEntityAdded(discoveredEntities[i]);
                }
            }

            return DiscoverRelationships(entity);
        }

        private Entity DiscoverRelationships(Entity entity)
        {
            if (entity.IsShadowEntity /* || entity.Model.IsIgnored(entity.ClrType) */ )
            {
                return entity;
            }

            var relationshipCandidates = FindRelationshipCandidates(entity);
            relationshipCandidates = RemoveIncompatibleRelationships(relationshipCandidates, entity);
            CreateRelationships(relationshipCandidates, entity);

            return entity;
        }

        private void CreateRelationships(IReadOnlyList<RelationshipCandidate> relationshipCandidates, Entity entity)
        {
            foreach (var relationshipCandidate in relationshipCandidates)
            {
                var navigationProperty = relationshipCandidate.NavigationProperties.SingleOrDefault();
                var inverseProperty = relationshipCandidate.InverseProperties.SingleOrDefault();
                if (inverseProperty == null)
                {
                    entity.AddManyToManyRelationship(relationshipCandidate.TargetEntity, navigationProperty);
                }
                else
                {
                    entity.AddManyToManyRelationship(relationshipCandidate.TargetEntity, navigationProperty, inverseProperty);
                }
            }
        }

        private IReadOnlyList<RelationshipCandidate> FindRelationshipCandidates(Entity entity)
        {
            var relationshipCandidates = new Dictionary<Type, RelationshipCandidate>();
            var navigationCandidates = GetNavigationCandidates(entity);
            foreach (var candidateTuple in navigationCandidates)
            {
                var navigationPropertyInfo = candidateTuple.Key;
                var targetClrType = candidateTuple.Value;

                //if (entity.IsIgnored(navigationPropertyInfo.Name))
                //{
                //    continue;
                //}

                var candidateTargetEntity = entity.Model.FindEntity(targetClrType);
                if (candidateTargetEntity == null)
                {
                    continue;
                }

                RelationshipCandidate existingCandidate;
                if (relationshipCandidates.TryGetValue(targetClrType, out existingCandidate))
                {
                    if (candidateTargetEntity != entity || !existingCandidate.InverseProperties.Contains(navigationPropertyInfo))
                    {
                        existingCandidate.NavigationProperties.Add(navigationPropertyInfo);
                    }

                    continue;
                }

                var navigations = new HashSet<PropertyInfo> { navigationPropertyInfo };

                var inverseCandidates = GetNavigationCandidates(candidateTargetEntity);
                var inverseNavigationCandidates = new HashSet<PropertyInfo>();
                foreach (var inverseCandidateTuple in inverseCandidates)
                {
                    var inversePropertyInfo = inverseCandidateTuple.Key;
                    var inverseTargetType = inverseCandidateTuple.Value;

                    if (inverseTargetType != entity.ClrType || navigationPropertyInfo == inversePropertyInfo /* || candidateTargetEntity.IsIgnored(inversePropertyInfo.Name) */ )
                    {
                        continue;
                    }

                    inverseNavigationCandidates.Add(inversePropertyInfo);
                }

                relationshipCandidates[targetClrType] = new RelationshipCandidate(candidateTargetEntity, navigations, inverseNavigationCandidates);
            }

            return relationshipCandidates.Values.ToList();
        }

        private IReadOnlyList<RelationshipCandidate> RemoveIncompatibleRelationships(IReadOnlyList<RelationshipCandidate> relationshipCandidates, Entity entity)
        {
            var filteredRelationshipCandidates = new List<RelationshipCandidate>();
            foreach (var relationshipCandidate in relationshipCandidates)
            {
                var targetEntity = relationshipCandidate.TargetEntity;
                int nbNavigationProperties = relationshipCandidate.NavigationProperties.Count;
                var revisitNavigations = true;
                while (revisitNavigations)
                {
                    revisitNavigations = false;
                    foreach (var navigationProperty in relationshipCandidate.NavigationProperties)
                    {
                        var existingNavigation = entity.FindNavigation(navigationProperty.Name);
                        if (existingNavigation != null)
                        {
                            relationshipCandidate.NavigationProperties.Remove(navigationProperty);
                            revisitNavigations = true;
                            break;
                        }

                        var navigationPropertyAttribute = navigationProperty.GetCustomAttribute<RelationshipAttribute>();
                        var compatibleInverseProperties = new List<PropertyInfo>();
                        var revisitInverses = true;
                        while (revisitInverses)
                        {
                            revisitInverses = false;
                            foreach (var inversePropertyInfo in relationshipCandidate.InverseProperties)
                            {
                                if (navigationPropertyAttribute != null &&
                                    navigationPropertyAttribute.InverseProperty.Equals(inversePropertyInfo.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    compatibleInverseProperties.Add(inversePropertyInfo);
                                    relationshipCandidate.InverseProperties.Remove(inversePropertyInfo);
                                    revisitInverses = true;
                                    break;
                                }

                                var inversePropertyAttribute = inversePropertyInfo.GetCustomAttribute<RelationshipAttribute>();
                                if (inversePropertyAttribute != null &&
                                    inversePropertyAttribute.InverseProperty.Equals(navigationProperty.Name, StringComparison.OrdinalIgnoreCase))
                                {
                                    compatibleInverseProperties.Add(inversePropertyInfo);
                                    relationshipCandidate.InverseProperties.Remove(inversePropertyInfo);
                                    revisitInverses = true;
                                    break;
                                }

                                if (relationshipCandidate.NavigationProperties.Count == 1 && relationshipCandidate.InverseProperties.Count == 1)
                                {
                                    compatibleInverseProperties.Add(inversePropertyInfo);
                                    relationshipCandidate.InverseProperties.Remove(inversePropertyInfo);
                                    revisitInverses = true;
                                    break;
                                }
                            }
                        }

                        if (compatibleInverseProperties.Count == 0 &&
                           ((navigationPropertyAttribute != null && string.IsNullOrWhiteSpace(navigationPropertyAttribute.InverseProperty)) || nbNavigationProperties == 1))
                        {
                            relationshipCandidate.NavigationProperties.Remove(navigationProperty);
                            filteredRelationshipCandidates.Add(new RelationshipCandidate(targetEntity, new HashSet<PropertyInfo> { navigationProperty }, new HashSet<PropertyInfo>()));

                            if ((relationshipCandidate.TargetEntity == entity) && (relationshipCandidate.InverseProperties.Count > 0))
                            {
                                var nextSelfRefCandidate = relationshipCandidate.InverseProperties.First();
                                relationshipCandidate.NavigationProperties.Add(nextSelfRefCandidate);
                                relationshipCandidate.InverseProperties.Remove(nextSelfRefCandidate);
                            }

                            revisitNavigations = true;
                            break;
                        }

                        if (compatibleInverseProperties.Count == 1)
                        {
                            var inverseProperty = compatibleInverseProperties[0];
                            relationshipCandidate.NavigationProperties.Remove(navigationProperty);
                            relationshipCandidate.InverseProperties.Remove(inverseProperty);
                            filteredRelationshipCandidates.Add(new RelationshipCandidate(targetEntity, new HashSet<PropertyInfo> { navigationProperty }, new HashSet<PropertyInfo> { inverseProperty }));

                            if ((relationshipCandidate.TargetEntity == entity) && (relationshipCandidate.InverseProperties.Count > 0))
                            {
                                var nextSelfRefCandidate = relationshipCandidate.InverseProperties.First();
                                relationshipCandidate.NavigationProperties.Add(nextSelfRefCandidate);
                                relationshipCandidate.InverseProperties.Remove(nextSelfRefCandidate);
                            }

                            revisitNavigations = true;
                            break;
                        }
                    }
                }

                if (relationshipCandidate.NavigationProperties.Count > 0)
                {
                    AddAmbiguousNavigations(entity, relationshipCandidate.NavigationProperties);
                }
            }

            return filteredRelationshipCandidates;
        }

        private SortedDictionary<PropertyInfo, Type> GetNavigationCandidates(Entity entity)
        {
            var navigationCandidates = entity.FindAnnotation(NAVIGATION_CANDIDATES_ANNOTATION_NAME)?.Value as SortedDictionary<PropertyInfo, Type>;
            if (navigationCandidates == null)
            {
                navigationCandidates = new SortedDictionary<PropertyInfo, Type>(PropertyInfoNameComparer.Instance);
                foreach (var propertyInfo in entity.ClrType.GetRuntimeProperties().OrderBy(p => p.Name))
                {
                    var targetType = propertyInfo.FindCandidateNavigationPropertyType();
                    if (targetType != null)
                    {
                        navigationCandidates[propertyInfo] = targetType;
                    }
                }

                SetNavigationCandidates(entity, navigationCandidates);
            }

            return navigationCandidates;
        }

        private void SetNavigationCandidates(Entity entity, SortedDictionary<PropertyInfo, Type> navigationCandidates) 
            => entity.SetAnnotation(NAVIGATION_CANDIDATES_ANNOTATION_NAME, navigationCandidates);

        private SortedDictionary<PropertyInfo, Type> GetAmbigousNavigations(Entity entity) => entity.FindAnnotation(AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME)?.Value as SortedDictionary<PropertyInfo, Type>;

        private void AddAmbiguousNavigations(Entity entity, IEnumerable<PropertyInfo> navigationProperties)
        {
            var ambiguousNavigations = GetAmbigousNavigations(entity) ?? new SortedDictionary<PropertyInfo, Type>(PropertyInfoNameComparer.Instance);
            entity.RemoveAnnotation(AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME);

            navigationProperties.ToList().ForEach(x => ambiguousNavigations.Add(x, entity.ClrType));
            entity.AddAnnotation(AMBIGUOUS_NAVIGATIONS_ANNOTATION_NAME, ambiguousNavigations);
        }

        private bool RemoveAmbiguous(Entity entity, PropertyInfo navigationProperty)
        {
            var ambigousNavigations = GetAmbigousNavigations(entity);
            if (ambigousNavigations != null)
            {
                if (ambigousNavigations.Remove(navigationProperty))
                {
                    AddAmbiguousNavigations(entity, ambigousNavigations.Keys);
                    return true;
                }
            }

            return false;
        }

        private class RelationshipCandidate
        {
            public RelationshipCandidate(Entity targetEntity, HashSet<PropertyInfo> navigations, HashSet<PropertyInfo> inverseNavigations)
            {
                TargetEntity = targetEntity;
                NavigationProperties = navigations;
                InverseProperties = inverseNavigations;
            }

            public Entity TargetEntity { get; }
            public HashSet<PropertyInfo> NavigationProperties { get; }
            public HashSet<PropertyInfo> InverseProperties { get; set; }
        }

        private class PropertyInfoNameComparer : IComparer<PropertyInfo>
        {
            public static readonly PropertyInfoNameComparer Instance = new PropertyInfoNameComparer();

            private PropertyInfoNameComparer()
            {
            }

            public int Compare(PropertyInfo x, PropertyInfo y) => StringComparer.Ordinal.Compare(x.Name, y.Name);
        }
    }
}
