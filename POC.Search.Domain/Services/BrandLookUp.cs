using POC.Search.Domain.Comparers;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace POC.Search.Domain.Services
{
    [Serializable]
    [DataContract]
    public class BrandLookUp
    {
        [DataMember(Order = 1)]
        public IDictionary<BrandId, BrandLookUpValue> Brands { get; private set; }

        public BrandLookUp()
        {
            this.Brands = new ConcurrentDictionary<BrandId, BrandLookUpValue>(new BrandIdComparer());
        }
    }
}
