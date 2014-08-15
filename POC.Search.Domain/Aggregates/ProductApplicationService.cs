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
    public class ProductApplicationService : IApplicationService
    {
        readonly IEventStore _eventStore;

        public ProductApplicationService(IEventStore eventStore)
        {
            Ensure.NotNull(eventStore, "eventStore");
            this._eventStore = eventStore;
        }
        public void Update(ProductId id, Action<Product> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Product product = new Product(eventStream.Events);

                execute(product);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, product.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var productEvent in product.Changes)
                    {
                        foreach (var foodEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(productEvent, foodEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}", productEvent, foodEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    _eventStore.AppendToStream(id, ex.ActualVersion, product.Changes);
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
        public void When(AddNewProduct c)
        {
            Update(c.Id, a => a.AddNewProduct(c.Id, c.Name, c.Description,c.Brand, c.Facets));
        }
        public void When(AddFacetToProduct c)
        {
            Update(c.Id, a => a.AddFacetToProduct(c.Id, c.Facet));
        }
    }
}
