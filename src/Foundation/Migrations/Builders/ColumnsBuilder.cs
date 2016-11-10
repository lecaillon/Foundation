using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
