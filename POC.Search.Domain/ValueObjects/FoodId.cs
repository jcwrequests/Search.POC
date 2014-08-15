using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class FoodId : IIdentity, IEqualityComparer<FoodId>
    {
        public FoodId(string value)
        {
            Ensure.NotNull(value, "value");
            this.Value = value;
        }
        public string Value { get; private set; }
        public override string ToString()
        {
            return string.Format("FoodId{0}",this.Value);
        }

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
