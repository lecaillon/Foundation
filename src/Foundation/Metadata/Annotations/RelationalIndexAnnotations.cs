using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation.Utilities;

namespace Foundation.Metadata.Annotations
{
    public class RelationalIndexAnnotations : IRelationalIndexAnnotations
    {
        protected const string DefaultIndexNamePrefix = "IX";

        public RelationalIndexAnnotations(Index index)
        {
            Check.NotNull(index, nameof(index));

            Index = index;
        }

        protected virtual Index Index { get; }

        protected virtual Annotable Metadata => Index;

        public virtual string Name
        {
            get { return (string)Metadata[RelationalAnnotationNames.Name] ?? GetDefaultName(); }
            set { Metadata[RelationalAnnotationNames.Name] = Check.NullButNotEmpty(value, nameof(value)); }
        }

        protected virtual string GetDefaultName()
        {
            return GetDefaultIndexName(new RelationalEntityAnnotations(Index.DeclaringEntity).TableName,
                                       Index.Properties.Select(p => new RelationalPropertyAnnotations(p).ColumnName));
        }

        public static string GetDefaultIndexName(string tableName, IEnumerable<string> propertyNames)
        {
            Check.NotEmpty(tableName, nameof(tableName));
            Check.NotNull(propertyNames, nameof(propertyNames));

            return new StringBuilder()
                .Append(DefaultIndexNamePrefix)
                .Append("_")
                .Append(tableName)
                .Append("_")
                .AppendJoin(propertyNames, "_")
                .ToString();
        }
    }
}
