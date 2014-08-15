using POC.Search.Domain.Contracts;
using POC.Search.Domain.Exceptions;
using POC.Search.Domain.Infrastructure;
using POC.Search.Domain.Storage;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public sealed class FacetApplicationService : IApplicationService
    {
        readonly IEventStore _eventStore;

        public FacetApplicationService(IEventStore eventStore)
        {
            Ensure.NotNull(eventStore, "eventStore");
            this._eventStore = eventStore;
        }
        public void When(AddNewFacet c)
        {
            Update(c.Facet.Id, a => a.AddNewFacet(c.Facet));
        }
        public void Update(FacetId id, Action<Facet> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Facet facet = new Facet(eventStream.Events);

                execute(facet);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, facet.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var facetEvent in facet.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(facetEvent, actualEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}", facetEvent, actualEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    _eventStore.AppendToStream(id, ex.ActualVersion, facet.Changes);
                }
            }
        }
        static bool ConflictsWith(IEvent x, IEvent y)
        {
            return x.GetType() == y.GetType();
        }

        public void Execute(ICommand cmd)
        {
            ((dynamic)this).When((dynamic)cmd);
        } 
    }
}
