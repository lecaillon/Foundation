using System;
using System.Collections.Generic;
using Foundation.Metadata;
using Foundation.Utilities;

namespace Foundation.Storage
{
    public class RelationalParameterBuilder : IRelationalParameterBuilder
    {
        private readonly List<IRelationalParameter> _parameters = new List<IRelationalParameter>();

        public RelationalParameterBuilder(IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(typeMapper, nameof(typeMapper));

            TypeMapper = typeMapper;
        }

        public virtual IReadOnlyList<IRelationalParameter> Parameters => _parameters;

        protected virtual IRelationalTypeMapper TypeMapper { get; }

        public virtual void AddParameter(string invariantName, string name)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));

            _parameters.Add(new DynamicRelationalParameter(invariantName, name, TypeMapper));
        }

        public virtual void AddParameter(string invariantName, string name, RelationalTypeMapping typeMapping, bool nullable)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(typeMapping, nameof(typeMapping));

            _parameters.Add(new TypeMappedRelationalParameter(invariantName, name, typeMapping, nullable));
        }

        public virtual void AddParameter(string invariantName, string name, Property property)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(property, nameof(property));

            _parameters.Add(new TypeMappedRelationalParameter(invariantName, name, TypeMapper.GetMapping(property), property.IsNullable));
        }

        public virtual void AddCompositeParameter(string invariantName, Action<IRelationalParameterBuilder> buildAction)
        {
            Check.NotEmpty(invariantName, nameof(invariantName));
            Check.NotNull(buildAction, nameof(buildAction));

            var innerList = new RelationalParameterBuilder(TypeMapper);

            buildAction(innerList);

            if (innerList.Parameters.Count > 0)
            {
                _parameters.Add(new CompositeRelationalParameter(invariantName, innerList.Parameters));
            }
        }

        public virtual void AddPropertyParameter(string invariantName, string name, Property property)
        {
            throw new NotImplementedException();
        }
    }
}
