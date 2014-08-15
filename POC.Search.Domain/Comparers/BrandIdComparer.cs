using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Comparers
{
    [Serializable]
    public class BrandIdComparer : IEqualityComparer<BrandId>
    {
        public bool Equals(BrandId x, BrandId y)
        {
            return x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(BrandId obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
