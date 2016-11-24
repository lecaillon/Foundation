using System;
using System.Collections.Generic;
using System.Data.Common;
using Foundation.Utilities;

namespace Foundation.Storage
{
    public class CompositeRelationalParameter : IRelationalParameter
    {
        public CompositeRelationalParameter(string invariantName, IReadOnlyList<IRelationalParameter> relationalParameters)

        {
            Check.NotNull(invariantName, nameof(invariantName));
            Check.NotNull(relationalParameters, nameof(relationalParameters));

            InvariantName = invariantName;
            RelationalParameters = relationalParameters;
        }

        public virtual string InvariantName { get; }

        public virtual IReadOnlyList<IRelationalParameter> RelationalParameters { get; }

        public virtual void AddDbParameter(DbCommand command, object value)
        {
            Check.NotNull(command, nameof(command));
            Check.NotNull(value, nameof(value));

            var innerValues = value as object[];

            if (innerValues != null)
            {
                if (innerValues.Length < RelationalParameters.Count)
                {
                    throw new InvalidOperationException(ResX.MissingParameterValue(RelationalParameters[innerValues.Length].InvariantName));
                }

                for (var i = 0; i < RelationalParameters.Count; i++)
                {
                    RelationalParameters[i].AddDbParameter(command, innerValues[i]);
                }
            }
            else
            {
                throw new InvalidOperationException(ResX.ParameterNotObjectArray(InvariantName));
            }
        }
    }
}
