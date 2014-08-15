using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class BrandId : IIdentity, IEqualityComparer<BrandId>
    {
        public BrandId(string value)
        {
            Ensure.NotNull(value, "value");
            this.Value = value;
        }
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("BrandId{0}", this.Value);
        }
        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(typeof(BrandId))) return false;
            BrandId other = (BrandId)obj;
            return Equals(this, other);
        }
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
