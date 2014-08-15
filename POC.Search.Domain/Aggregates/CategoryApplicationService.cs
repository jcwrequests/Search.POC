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
    public sealed class CategoryApplicationService : IApplicationService
    {
        readonly IEventStore _eventStore;

        public CategoryApplicationService(IEventStore eventStore)
        {
            Ensure.NotNull(eventStore, "eventStore");
            this._eventStore = eventStore;
        }
        public void When(AddNewCategory c)
        {
            Update(c.Id,a => a.AddNewCategory(c.Id,c.Name,c.Parent,c.Aliases));
        }
        public void When(AddAliasToCategory c)
        {
            Update(c.Id, a => a.AddNewAlias(c.Id, c.Alias));
        }
        public void Update(CategoryId id, Action<Category> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Category category = new Category(eventStream.Events);

                execute(category);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, category.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var categoryEvent in category.Changes)
                    {
                        foreach (var actualEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(categoryEvent, actualEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}", categoryEvent, actualEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    _eventStore.AppendToStream(id, ex.ActualVersion, category.Changes);
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
