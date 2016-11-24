using System.Diagnostics;
using Foundation.Utilities;
using Microsoft.Extensions.Logging;

namespace Foundation.Storage
{
    public class RelationalCommandBuilder : IRelationalCommandBuilder
    {
        private readonly ILogger _logger;
        private readonly DiagnosticSource _diagnosticSource;

        private readonly IndentedStringBuilder _commandTextBuilder = new IndentedStringBuilder();

        public RelationalCommandBuilder(ILogger logger, DiagnosticSource diagnosticSource, IRelationalTypeMapper typeMapper)
        {
            Check.NotNull(logger, nameof(logger));
            Check.NotNull(diagnosticSource, nameof(diagnosticSource));
            Check.NotNull(typeMapper, nameof(typeMapper));

            _logger = logger;
            _diagnosticSource = diagnosticSource;
            ParameterBuilder = new RelationalParameterBuilder(typeMapper);
        }

        public virtual IRelationalParameterBuilder ParameterBuilder { get; }

        public virtual IRelationalCommand Build() => new RelationalCommand(_logger, _diagnosticSource, _commandTextBuilder.ToString(), ParameterBuilder.Parameters);

        public override string ToString() => _commandTextBuilder.ToString();
    }
}
