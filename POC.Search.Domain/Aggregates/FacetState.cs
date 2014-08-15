using POC.Search.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class FacetState
    {
        public FacetState(IEnumerable<IEvent> events)
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

        public void When(NewFacetAdded e)
        {
            this.Facet = e.Facet; 
        }
        public ValueObjects.Facet Facet { get; private set; }
    }
}
