using System.Data.Common;
using Foundation.Utilities;

namespace Foundation.Storage
{
    public class DynamicRelationalParameter : IRelationalParameter
    {
        private readonly IRelationalTypeMapper _typeMapper;

        public DynamicRelationalParameter(string invariantName, string name, IRelationalTypeMapper typeMapper)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(typeMapper, nameof(typeMapper));

            InvariantName = invariantName;
            Name = name;
            _typeMapper = typeMapper;
        }

        public virtual string InvariantName { get; }

        public virtual string Name { get; }

        public virtual void AddDbParameter(DbCommand command, object value)
        {
            Check.NotNull(command, nameof(command));

            if (value == null)
            {
                command.Parameters.Add(_typeMapper.GetMappingForValue(null).CreateParameter(command, Name, null));

                return;
            }

            var dbParameter = value as DbParameter;

            if (dbParameter != null)
            {
                command.Parameters.Add(dbParameter);

                return;
            }

            var type = value.GetType();

            command.Parameters.Add(_typeMapper.GetMapping(type).CreateParameter(command, Name, value, type.IsNullableType()));
        }
    }
}
