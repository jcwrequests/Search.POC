using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Comparers
{
    [Serializable]
    public class CategoryIdComparer : IEqualityComparer<CategoryId>
    {
        public bool Equals(CategoryId x, CategoryId y)
        {
            return x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(CategoryId obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
