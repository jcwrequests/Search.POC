using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class ProductId : IIdentity, IEqualityComparer<ProductId>
    {
        public ProductId(string value)
        {
            Ensure.NotNull(value, "value");
            this.Value = value;
        }

        public string Value { get; private set; }
        public override string ToString()
        {
            return string.Format("ProductId{0}", this.Value);
        }

        public bool Equals(ProductId x, ProductId y)
        {
            return x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ProductId obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
