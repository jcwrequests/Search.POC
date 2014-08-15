using POC.Search.Domain.Contracts;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class BrandState
    {
        public BrandState(IEnumerable<IEvent> events)
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

        public void When(BrandAdded e)
        {
            this.Id = e.Id;
            this.Name = e.Name;
            this.BrandTerms = e.BrandTerms;
        }
        public void When(TermAddedToBrand e)
        {
            List<BrandTerm> terms = new List<BrandTerm>();
            if (this.BrandTerms != null) terms.AddRange(this.BrandTerms);
            terms.Add(e.BrandTerm);
            this.BrandTerms = terms.ToArray();
        }
        public BrandId Id { get; private set; }
        public string Name { get; private set; }
        public BrandTerm[] BrandTerms { get; private set; }
        
    }
}
