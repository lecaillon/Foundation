using System.Data.Common;
using Foundation.Utilities;

namespace Foundation.Storage
{
    public class TypeMappedRelationalParameter : IRelationalParameter
    {
        public TypeMappedRelationalParameter(string invariantName, string name, RelationalTypeMapping relationalTypeMapping, bool? nullable)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(relationalTypeMapping, nameof(relationalTypeMapping));

            InvariantName = invariantName;
            Name = name;
            RelationalTypeMapping = relationalTypeMapping;
            IsNullable = nullable;
        }

        public virtual string InvariantName { get; }

        public virtual string Name { get; }

        // internal for testing
        internal RelationalTypeMapping RelationalTypeMapping { get; }

        // internal for testing
        internal bool? IsNullable { get; }

        public virtual void AddDbParameter(DbCommand command, object value)
        {
            Check.NotNull(command, nameof(command));

            command.Parameters.Add(RelationalTypeMapping.CreateParameter(command, Name, value, IsNullable));
        }
    }
}
