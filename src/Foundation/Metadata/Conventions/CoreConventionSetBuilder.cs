namespace Foundation.Metadata.Conventions
{
    public class CoreConventionSetBuilder
    {
        public virtual ConventionSet CreateConventionSet()
        {
            var conventionSet = new ConventionSet();

            conventionSet.EntityAddedConventions.Add(new BaseTypeDiscoveryConvention());
            conventionSet.EntityAddedConventions.Add(new PropertyDiscoveryConvention());
            conventionSet.EntityAddedConventions.Add(new KeyDiscoveryConvention());
            conventionSet.EntityAddedConventions.Add(new RelationshipDiscoveryConvention());

            return conventionSet;
        }
    }
}
