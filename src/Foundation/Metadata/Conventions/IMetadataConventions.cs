namespace Foundation.Metadata.Conventions
{
    public interface IEntityConvention
    {
        Entity Apply(Entity entity);
    }

    public interface IBaseTypeConvention
    {
        bool Apply(Entity entity);
    }

    public interface IPropertyConvention
    {
        Property Apply(Property property);
    }

    public interface IPrimaryKeyConvention
    {
        bool Apply(Key primaryKey, Key previousPrimaryKey);
    }

    public interface IKeyConvention
    {
        Key Apply(Key key);
    }

    public interface IForeignKeyConvention
    {
        ForeignKey Apply(ForeignKey foreignKey);
    }
}
