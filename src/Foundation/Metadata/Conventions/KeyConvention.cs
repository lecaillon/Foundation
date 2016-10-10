using System;

namespace Foundation.Metadata.Conventions
{
    public class KeyConvention : IPrimaryKeyConvention
    {
        public bool Apply(Key primaryKey, Key previousPrimaryKey)
        {
            if (previousPrimaryKey != null)
            {
                // TODO : TO BE CONTINUED

            }

            return true;
        }
    }
}
