using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class FoodTerm
    {
        public FoodTerm(FoodId foodId, string term)
        {
            Ensure.NotNull(foodId, "foodId");
            Ensure.NotNull(term, "term");
            this.FoodId = foodId;
            this.Term = term;
        }
        public FoodId FoodId {get; private set;}
        public string Term {get;private set;}
    }
}
