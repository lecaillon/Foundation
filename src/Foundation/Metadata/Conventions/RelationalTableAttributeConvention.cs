using System.ComponentModel.DataAnnotations.Schema;

namespace Foundation.Metadata.Conventions
{
    public class RelationalTableAttributeConvention : EntityAttributeConventionBase<TableAttribute>
    {
        public override Entity Apply(Entity entity, TableAttribute attribute)
        {
            if (!string.IsNullOrWhiteSpace(attribute.Schema))
            {
                //entity.Relational().ToTable(attribute.Name, attribute.Schema);
            }

            if (!string.IsNullOrWhiteSpace(attribute.Name))
            {
                entity.Relational().TableName = attribute.Name;
            }

            return entity;
        }
    }
}
