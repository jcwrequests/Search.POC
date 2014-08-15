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
    public sealed class BrandApplicationService : IApplicationService
    {
        readonly IEventStore _eventStore;

        public BrandApplicationService(IEventStore eventStore)
        {
            Ensure.NotNull(eventStore, "eventStore");
            this._eventStore = eventStore;
        }
        public void Update(BrandId id,Action<Brand> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Brand brand = new Brand(eventStream.Events);

                execute(brand);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, brand.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var brandEvent in brand.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(brandEvent, actualEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}", brandEvent, actualEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    _eventStore.AppendToStream(id, ex.ActualVersion, brand.Changes);
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

        public void When(AddBrand c)
        {
            Update(c.Id, a => a.AddNewBrand(c.Id, c.Name, c.BrandTerms));
        }
        public void When(AddTermToBrand c)
        {
            Update(c.Id, a => a.AddNewBrandTerm(c.Id, c.BrandTerm));
        }
    }
}
