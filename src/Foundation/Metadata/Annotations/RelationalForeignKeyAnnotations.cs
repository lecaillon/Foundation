using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalForeignKeyAnnotations : IRelationalForeignKeyAnnotations
    {
        protected const string DefaultForeignKeyNamePrefix = "FK";

        public RelationalForeignKeyAnnotations(ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            ForeignKey = foreignKey;
        }

        protected virtual ForeignKey ForeignKey { get; }

        protected virtual Annotable Metadata => ForeignKey;

        public virtual string Name
        {
            get { return (string)Metadata[RelationalAnnotationNames.Name] ?? GetDefaultName(); }
            set { Metadata[RelationalAnnotationNames.Name] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        protected virtual string GetDefaultName()
        {
            return GetDefaultForeignKeyName(new RelationalEntityAnnotations(ForeignKey.DeclaringEntity).TableName,
                                            new RelationalEntityAnnotations(ForeignKey.PrincipalEntity).TableName,
                                            ForeignKey.Properties.Select(p => new RelationalPropertyAnnotations(p).ColumnName));
        }

        public static string GetDefaultForeignKeyName(string dependentTableName, string principalTableName, IEnumerable<string> dependentEndPropertyNames)
        {
            Check.NotEmpty(dependentTableName, nameof(dependentTableName));
            Check.NotEmpty(principalTableName, nameof(principalTableName));
            Check.NotNull(dependentEndPropertyNames, nameof(dependentEndPropertyNames));

            return new StringBuilder()
                .Append(DefaultForeignKeyNamePrefix)
                .Append("_")
                .Append(dependentTableName)
                .Append("_")
                .Append(principalTableName)
                .Append("_")
                .AppendJoin(dependentEndPropertyNames, "_")
                .ToString();
        }
    }
}
