using System;
using System.Collections.Generic;
using Foundation.Metadata;

namespace Foundation.Storage
{
    /// <summary>
    ///     Builds a collection of <see cref="IRelationalParameter" />.
    /// </summary>
    public interface IRelationalParameterBuilder
    {
        /// <summary>
        ///     The collection of parameters.
        /// </summary>
        IReadOnlyList<IRelationalParameter> Parameters { get; }

        /// <summary>
        ///     Adds a parameter.
        /// </summary>
        /// <param name="invariantName">
        ///     The key that identifies this parameter. Note that <see cref="IRelationalParameter" /> just represents a
        ///     placeholder for a parameter and not the actual value. This is because the same command can be
        ///     reused multiple times with different parameter values.
        /// </param>
        /// <param name="name">
        ///     The name to be used for the parameter when the command is executed against the database.
        /// </param>
        void AddParameter(string invariantName, string name);

        /// <summary>
        ///     Adds a parameter.
        /// </summary>
        /// <param name="invariantName">
        ///     The key that identifies this parameter. Note that <see cref="IRelationalParameter" /> just represents a
        ///     placeholder for a parameter and not the actual value. This is because the same command can be
        ///     reused multiple times with different parameter values.
        /// </param>
        /// <param name="name">
        ///     The name to be used for the parameter when the command is executed against the database.
        /// </param>
        /// <param name="typeMapping">
        ///     The type mapping for the property that values for this parameter will come from.
        /// </param>
        /// <param name="nullable">
        ///     A value indicating whether the parameter can contain null values.
        /// </param>
        void AddParameter(string invariantName, string name, RelationalTypeMapping typeMapping, bool nullable);

        /// <summary>
        ///     Adds a parameter.
        /// </summary>
        /// <param name="invariantName">
        ///     The key that identifies this parameter. Note that <see cref="IRelationalParameter" /> just represents a
        ///     placeholder for a parameter and not the actual value. This is because the same command can be
        ///     reused multiple times with different parameter values.
        /// </param>
        /// <param name="name">
        ///     The name to be used for the parameter when the command is executed against the database.
        /// </param>
        /// <param name="property"></param>
        void AddParameter(string invariantName, string name, Property property);

        /// <summary>
        ///     Adds a parameter that is ultimately represented as multiple <see cref="DbParameter" />s in the
        ///     final command.
        /// </summary>
        /// <param name="invariantName">
        ///     The key that identifies this parameter. Note that <see cref="IRelationalParameter" /> just represents a
        ///     placeholder for a parameter and not the actual value. This is because the same command can be
        ///     reused multiple times with different parameter values.
        /// </param>
        /// <param name="buildAction">
        ///     The action to add the multiple parameters that this placeholder represents.
        /// </param>
        void AddCompositeParameter(string invariantName, Action<IRelationalParameterBuilder> buildAction);

        /// <summary>
        ///     Adds a parameter.
        /// </summary>
        /// <param name="invariantName">
        ///     The key that identifies this parameter. Note that <see cref="IRelationalParameter" /> just represents a
        ///     placeholder for a parameter and not the actual value. This is because the same command can be
        ///     reused multiple times with different parameter values.
        /// </param>
        /// <param name="name">
        ///     The name to be used for the parameter when the command is executed against the database.
        /// </param>
        /// <param name="property">
        ///     The property that values for this parameter will come from.
        /// </param>
        void AddPropertyParameter(string invariantName, string name, Property property);
    }
}
