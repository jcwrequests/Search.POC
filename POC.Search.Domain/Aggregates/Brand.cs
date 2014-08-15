using POC.Search.Domain.Contracts;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class Brand
    {
        public Brand(IEnumerable<IEvent> events)
        {
            this._state = new BrandState(events);
        }
        public readonly IList<IEvent> Changes = new List<IEvent>();
        readonly BrandState _state;

        void Apply(IEvent e)
        {
            _state.Mutate(e);
            Changes.Add(e);
        }
        public void AddNewBrand(BrandId id,string name,BrandTerm[] brandTerms)
        {
            if (_state.Id != null) throw new Exception("Brand already added");

            Apply(new BrandAdded(id, name, brandTerms));
        }
        public void AddNewBrandTerm(BrandId id, BrandTerm brandTerm)
        {
            if (_state.Id == null) throw new Exception("Can not add a Brand Term before it as been created");
            if (_state.Id != id) throw new Exception("Invalid Id");

            Apply(new TermAddedToBrand(id, brandTerm));
        }
    }
}
