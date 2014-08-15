using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class FoodState
    {
        public FoodState(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void Mutate(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
        }
        
        public void When(NewFoodAdded e)
        {
            this.Category = e.Category;
            this.Id = e.Id;
            this.Name = e.Name;
            this.FoodTerms = e.FoodTerms;
        }

        public void  When(NewFoodTermAddedToFood e)
        {
            List<FoodTerm> terms = new List<FoodTerm>();
            if (this.FoodTerms != null) terms.AddRange(this.FoodTerms);
            terms.Add(e.FoodTerm);
            this.FoodTerms = terms.ToArray();
        }
        public CategoryId Category { get; private set; }
        public FoodId Id { get; private set; }
        public string Name { get; private set; }
        public FoodTerm[] FoodTerms { get; private set; }
    }
}
