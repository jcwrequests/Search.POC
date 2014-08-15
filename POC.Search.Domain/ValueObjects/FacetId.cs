using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class FacetId : IIdentity , IEqualityComparer<Facet>
    {
        public FacetId(string value)
        {
            Ensure.NotNull(value, "value");
            this.Value = value;
        }
        public string Value { get; private set; }
        public override string ToString()
        {
            return string.Format("FacetId{0}",this.Value);
        }

        public bool Equals(Facet x, Facet y)
        {
            return x.Id.Value.Equals(y.Id.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(Facet obj)
        {
            return obj.Id.Value.GetHashCode();
        }
    }
}
