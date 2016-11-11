using System;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelationshipAttribute : Attribute
    {
        public RelationshipAttribute(string associationTableName, string inverseProperty = "")
        {
            Check.NotEmpty(associationTableName, nameof(associationTableName));

            AssociationTableName = associationTableName;
            InverseProperty = inverseProperty;
        }

        public string AssociationTableName { get; set; }

        public string InverseProperty { get; set; }
    }
}
