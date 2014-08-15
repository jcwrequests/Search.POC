using POC.Search.Domain.Contracts;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class Facet
    {
        public readonly IList<IEvent> Changes = new List<IEvent>();
        readonly FacetState _state;

        public Facet(IEnumerable<IEvent> events)
        {
            this._state = new FacetState(events);
        }
        void Apply(IEvent e)
        {
            _state.Mutate(e);
            Changes.Add(e);
        }
        public void AddNewFacet(ValueObjects.Facet facet)
        {
            if (_state.Facet != null) throw new Exception("Facet already created");
            Apply(new NewFacetAdded(facet));
        }
    }
}
