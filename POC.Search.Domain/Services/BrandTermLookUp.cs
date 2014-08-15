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
    public class BrandTermLookUp
    {
        [DataMember(Order = 1)]
        public IDictionary<string, List<BrandTermLookUpValue>> Brands { get; private set; }

        public BrandTermLookUp()
        {
            this.Brands = new ConcurrentDictionary<string, List<BrandTermLookUpValue>>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
