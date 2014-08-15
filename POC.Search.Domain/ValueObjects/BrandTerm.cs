using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class BrandTerm
    {
        public BrandTerm(BrandId brandId, string term)
        {
            Ensure.NotNull(brandId, "brandId");
            Ensure.NotNull(term, "term");
            this.BrandId = brandId;
            this.Term = term;
        }
        public BrandId BrandId { get; private set; }
        public string Term { get; private set; }
    }
}
