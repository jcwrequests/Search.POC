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
    public class CategoryLookUp
    {
        [DataMember(Order = 1)]
        public IDictionary<CategoryId, CategoryLookUpValue> Categories { get; private set; }

        public CategoryLookUp()
        {
            this.Categories = new ConcurrentDictionary<CategoryId, CategoryLookUpValue>(new CategoryIdComparer());
        }

    }
}
