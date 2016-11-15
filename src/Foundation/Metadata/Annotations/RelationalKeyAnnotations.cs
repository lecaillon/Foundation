using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalKeyAnnotations : IRelationalKeyAnnotations
    {
        protected const string DefaultPrimaryKeyNamePrefix = "PK";
        protected const string DefaultAlternateKeyNamePrefix = "AK";

        public RelationalKeyAnnotations(Key key)
        {
            Check.NotNull(key, nameof(key));

            Key = key;
        }

        protected virtual Key Key { get; }

        protected virtual Annotable Metadata => Key;

        public virtual string Name
        {
            get { return (string)Metadata[RelationalAnnotationNames.Name] ?? GetDefaultName(); }
            set { Metadata[RelationalAnnotationNames.Name] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        protected virtual string GetDefaultName()
        {
            return GetDefaultKeyName(new RelationalEntityAnnotations(Key.DeclaringEntity).TableName,
                                     Key.IsPrimaryKey(),
                                     Key.Properties.Select(p => new RelationalPropertyAnnotations(p).ColumnName));
        }

        public static string GetDefaultKeyName(string tableName, bool primaryKey, IEnumerable<string> propertyNames)
        {
            Check.NotEmpty(tableName, nameof(tableName));
            Check.NotNull(propertyNames, nameof(propertyNames));

            var builder = new StringBuilder();

            if (primaryKey)
            {
                builder
                    .Append(DefaultPrimaryKeyNamePrefix)
                    .Append("_")
                    .Append(tableName);
            }
            else
            {
                builder
                    .Append(DefaultAlternateKeyNamePrefix)
                    .Append("_")
                    .Append(tableName)
                    .Append("_")
                    .AppendJoin(propertyNames, "_");
            }

            return builder.ToString();
        }
    }
}
