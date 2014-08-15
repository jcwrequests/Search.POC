using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Contracts
{
    [Serializable]
    public class AddNewFacet : ICommand
    {
        public AddNewFacet(ValueObjects.Facet facet)
        {
            Ensure.NotNull(facet, "facet");
            this.Facet = facet;
        }
        public ValueObjects.Facet Facet { get; private set; }
    }
    [Serializable]
    public class NewFacetAdded : IEvent
    {
        public NewFacetAdded(ValueObjects.Facet facet)
        {
            Ensure.NotNull(facet, "facet");
            this.Facet = facet;
        }
        public ValueObjects.Facet Facet { get; private set; } 
    }
}
 