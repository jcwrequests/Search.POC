using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Comparers
{
     [Serializable]
    public class FoodIdComparer : IEqualityComparer<FoodId>
    {
        public bool Equals(FoodId x, FoodId y)
        {
            return x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(FoodId obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
