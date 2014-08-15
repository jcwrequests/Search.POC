using POC.Search.Domain.Contracts;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class Product
    { 
        public readonly IList<IEvent> Changes = new List<IEvent>();
        readonly ProductState _state;

        public Product(IEnumerable<IEvent> events)
        {
            this._state = new ProductState(events);
        }

        public void Apply(IEvent e)
        {
            _state.Mutate(e);
            Changes.Add(e);
        }
        public void AddNewProduct(ProductId id, string name, string description,BrandId brand, params ValueObjects.Facet[] facets)
        {
            if (_state.Id != null) throw new ArgumentException("This Product has already been added");
            Apply(new NewProductAdded(id,name,description,brand,facets));
        }
        public void AddFacetToProduct(ProductId id, ValueObjects.Facet facet)
        {
            if (_state.Id == null) throw new Exception("Can not add a Facet to a product that does not exists");
            List<ValueObjects.Facet> facets = new List<ValueObjects.Facet>();
            if (_state.Facets != null) facets.AddRange(_state.Facets);
            if (facets.Contains(facet)) throw new ArgumentException("Facet already exists");
            Apply(new FacetAddedToProduct(id, facet));
        }
    }
}
