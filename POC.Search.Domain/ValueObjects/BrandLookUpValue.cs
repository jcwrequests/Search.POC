using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class BrandLookUpValue
    {
        public BrandLookUpValue(BrandId id, string name,params BrandTerm[] terms)
        {
            this.Id = id;
            this.Name = name;
            this.Terms = terms;
        }
        public BrandId Id { get; private set; }
        public string Name { get; private set; }
        public BrandTerm[] Terms {get;private set;}
    }
}
