using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class BrandTermLookUpValue
    {
        public BrandTermLookUpValue(string term,BrandId brandId)
        {
            Ensure.NotNull(term, "term");
            Ensure.NotNull(brandId, "brandId");

            this.Term = term;
            this.BrandId = brandId;

        }
        public string Term { get; private set; }
        public BrandId BrandId { get; private set; }
    }
}
