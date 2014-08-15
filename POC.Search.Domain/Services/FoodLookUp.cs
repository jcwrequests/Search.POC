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
    public class FoodLookUp
    {
        [DataMember(Order = 1)]
        public IDictionary<FoodId, FoodLookUpValue> Foods { get; private set; }

        public FoodLookUp()
        {
            this.Foods = new ConcurrentDictionary<FoodId, FoodLookUpValue>(new FoodIdComparer());
        }
    }
}
