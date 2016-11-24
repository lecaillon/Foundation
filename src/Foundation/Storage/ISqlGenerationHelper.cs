using System.Text;

namespace Foundation.Storage
{
    /// <summary>
    ///     Provides services to help with generation of SQL commands.
    /// </summary>
    public interface ISqlGenerationHelper
    {
        /// <summary>
        ///     The terminator to be used for SQL statements.
        /// </summary>
        string StatementTerminator { get; }

        /// <summary>
        ///     The terminator to be used for batches of SQL statements.
        /// </summary>
        string BatchTerminator { get; }

        /// <summary>
        ///     Generates a valid parameter name for the given candidate name.
        /// </summary>
        /// <param name="name">
        ///     The candidate name for the parameter.
        /// </param>
        /// <returns> A valid name based on the candidate name. </returns>
        string GenerateParameterName(string name);

        /// <summary>
        ///     Writes a valid parameter name for the given candidate name.
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="name">
        ///     The candidate name for the parameter.
        /// </param>
        void GenerateParameterName(StringBuilder builder, string name);

        /// <summary>
        ///     Generates the SQL representation of a literal value.
        /// </summary>
        /// <param name="value"> The literal value. </param>
        /// <param name="typeMapping"> An optional type mapping that is used for this value. </param>
        /// <returns> The generated string. </returns>
        string GenerateLiteral(object value, RelationalTypeMapping typeMapping = null);

        /// <summary>
        ///     Writes the SQL representation of a literal value.
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="value"> The literal value. </param>
        /// <param name="typeMapping"> An optional type mapping that is used for this value. </param>
        void GenerateLiteral(StringBuilder builder, object value, RelationalTypeMapping typeMapping = null);

        /// <summary>
        ///     Generates the escaped SQL representation of a literal value.
        /// </summary>
        /// <param name="literal"> The value to be escaped. </param>
        /// <returns> The generated string. </returns>
        string EscapeLiteral(string literal);

        /// <summary>
        ///     Writes the escaped SQL representation of a literal value.
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="literal"> The value to be escaped. </param>
        void EscapeLiteral(StringBuilder builder, string literal);

        /// <summary>
        ///     Generates the escaped SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="identifier"> The identifier to be escaped. </param>
        /// <returns> The generated string. </returns>
        string EscapeIdentifier(string identifier);

        /// <summary>
        ///     Writes the escaped SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="identifier"> The identifier to be escaped. </param>
        void EscapeIdentifier(StringBuilder builder, string identifier);

        /// <summary>
        ///     Generates the delimited SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="identifier"> The identifier to delimit. </param>
        /// <returns> The generated string. </returns>
        string DelimitIdentifier(string identifier);

        /// <summary>
        ///     Writes the delimited SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="identifier"> The identifier to delimit. </param>
        void DelimitIdentifier(StringBuilder builder, string identifier);

        /// <summary>
        ///     Generates the delimited SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="name"> The identifier to delimit. </param>
        /// <param name="schema"> The schema of the identifier. </param>
        /// <returns> The generated string. </returns>
        string DelimitIdentifier(string name, string schema);

        /// <summary>
        ///     Writes the delimited SQL representation of an identifier (column name, table name, etc.).
        /// </summary>
        /// <param name="builder"> The <see cref="StringBuilder" /> to write generated string to. </param>
        /// <param name="name"> The identifier to delimit. </param>
        /// <param name="schema"> The schema of the identifier. </param>
        void DelimitIdentifier(StringBuilder builder, string name, string schema);
    }
}
