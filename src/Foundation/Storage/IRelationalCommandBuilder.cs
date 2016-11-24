namespace Foundation.Storage
{
    /// <summary>
    ///     Builds a command to be executed against a relational database.
    /// </summary>
    public interface IRelationalCommandBuilder
    {
        /// <summary>
        ///     Builds the parameters associated with this command.
        /// </summary>
        IRelationalParameterBuilder ParameterBuilder { get; }

        /// <summary>
        ///     Creates the command.
        /// </summary>
        /// <returns> The newly created command. </returns>
        IRelationalCommand Build();
    }
}
