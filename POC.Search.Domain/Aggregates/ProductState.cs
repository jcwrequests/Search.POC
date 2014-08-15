using POC.Search.Domain.ValueObjects;
using POC.Search.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class ProductState
    {
        public ProductState(IEnumerable<IEvent> events)
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

        public void When(NewProductAdded e)
        {
            this.Id = e.Id;
            this.Name = e.Name;
            this.Description = e.Description;
            this.Brand = e.Brand;
            this.Facets = e.Facets;
        }

        public void When(FacetAddedToProduct e)
        {
            List<ValueObjects.Facet> facets = new List<ValueObjects.Facet>();
            if (this.Facets != null) facets.AddRange(this.Facets);
            facets.Add(e.Facet);
            this.Facets = facets.ToArray();
        }

        public ProductId Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public BrandId Brand { get; private set; }
        public ValueObjects.Facet[] Facets { get; private set; }
    }
}
