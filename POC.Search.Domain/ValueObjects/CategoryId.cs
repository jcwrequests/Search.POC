using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class CategoryId : IIdentity, IEqualityComparer<CategoryId>
    {
        public CategoryId(string value)
        {
            Ensure.NotNull(value, "value");
            this.Value = value;
        }
        public string Value { get;private set; }
        public override string ToString()
        {
            return string.Format("CategoryId{0}",this.Value);
        }

        public bool Equals(CategoryId x, CategoryId y)
        {
            return x.Value.Equals(y.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(CategoryId obj)
        {
            return obj.GetHashCode();
        }
    }
}
