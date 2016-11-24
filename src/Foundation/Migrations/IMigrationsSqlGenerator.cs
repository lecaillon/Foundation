using System.Collections.Generic;
using Foundation.Metadata;
using Foundation.Migrations.Operations;

namespace Foundation.Migrations
{
    public interface IMigrationsSqlGenerator
    {
        IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, Model model = null);
    }
}
