using System.Data.Common;

namespace Foundation.Storage
{
    /// <summary>
    ///     A parameter in an <see cref="IRelationalCommand" />. Note that this interface just represents a
    ///     placeholder for a parameter and not the actual value. This is because the same command can be
    ///     reused multiple times with different parameter values.
    /// </summary>
    public interface IRelationalParameter
    {
        /// <summary>
        ///     The name of the parameter.
        /// </summary>
        string InvariantName { get; }

        /// <summary>
        ///     Adds the parameter as a <see cref="DbParameter" /> to a <see cref="DbCommand" />.
        /// </summary>
        /// <param name="command"> The command to add the parameter to. </param>
        /// <param name="value"> The value to be assigned to the parameter. </param>
        void AddDbParameter(DbCommand command, object value);
    }
}
