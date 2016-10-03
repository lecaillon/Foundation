using System;
using System.Collections.Generic;

namespace Foundation.Metadata.Internal
{
    public class ForeignKeyComparer : IEqualityComparer<ForeignKey>, IComparer<ForeignKey>
    {
        public static readonly ForeignKeyComparer Instance = new ForeignKeyComparer();

        public int Compare(ForeignKey x, ForeignKey y)
        {
            var result = PropertyListComparer.Instance.Compare(x.Properties, y.Properties);
            if (result != 0)
            {
                return result;
            }

            result = PropertyListComparer.Instance.Compare(x.PrincipalKey.Properties, y.PrincipalKey.Properties);
            if (result != 0)
            {
                return result;
            }

            return StringComparer.Ordinal.Compare(x.PrincipalEntity.Name, y.PrincipalEntity.Name);
        }

        public bool Equals(ForeignKey x, ForeignKey y) => Compare(x, y) == 0;

        public int GetHashCode(ForeignKey obj) =>
            unchecked(
                (((PropertyListComparer.Instance.GetHashCode(obj.PrincipalKey.Properties) * 397)
                 ^ PropertyListComparer.Instance.GetHashCode(obj.Properties)) * 397)
                 ^ StringComparer.Ordinal.GetHashCode(obj.PrincipalEntity.Name));
    }
}
