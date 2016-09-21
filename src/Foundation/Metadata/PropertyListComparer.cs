using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Metadata
{
    public class PropertyListComparer : IComparer<IReadOnlyList<Property>>, IEqualityComparer<IReadOnlyList<Property>>
    {
        public static readonly PropertyListComparer Instance = new PropertyListComparer();

        private PropertyListComparer() { }

        public int Compare(IReadOnlyList<Property> x, IReadOnlyList<Property> y)
        {
            var result = x.Count - y.Count;

            if (result != 0)
            {
                return result;
            }

            var index = 0;
            while ((result == 0) && (index < x.Count))
            {
                result = StringComparer.Ordinal.Compare(x[index].Name, y[index].Name);
                index++;
            }
            return result;
        }

        public bool Equals(IReadOnlyList<Property> x, IReadOnlyList<Property> y) => Compare(x, y) == 0;

        public int GetHashCode(IReadOnlyList<Property> obj) => obj.Aggregate(0, (hash, p) => unchecked((hash * 397) ^ p.GetHashCode()));
    }
}
