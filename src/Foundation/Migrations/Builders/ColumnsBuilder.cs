using Foundation.Migrations.Operations;
using Foundation.Utilities;

namespace Foundation.Migrations.Builders
{
    public class ColumnsBuilder
    {
        private readonly CreateTableOperation _createTableOperation;

        public ColumnsBuilder(CreateTableOperation createTableOperation)
        {
            Check.NotNull(createTableOperation, nameof(createTableOperation));

            _createTableOperation = createTableOperation;
        }
    }
}
