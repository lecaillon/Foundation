namespace Foundation.Metadata.Conventions
{
    public class CoreConventionSetBuilder
    {
        public virtual ConventionSet CreateConventionSet()
        {
            var conventionSet = new ConventionSet();

            conventionSet.EntityAddedStrictConventions.Add(new PropertyDiscoveryConvention());
            conventionSet.EntityAddedStrictConventions.Add(new KeyDiscoveryConvention());

            conventionSet.EntityAddedFullConventions.Add(new BaseTypeDiscoveryConvention());
            conventionSet.EntityAddedFullConventions.Add(new PropertyDiscoveryConvention());
            conventionSet.EntityAddedFullConventions.Add(new KeyDiscoveryConvention());
            conventionSet.EntityAddedFullConventions.Add(new RelationshipDiscoveryConvention());

            return conventionSet;
        }
    }
}
