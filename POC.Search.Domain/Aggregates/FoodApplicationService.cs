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
    public sealed class FoodApplicationService : IApplicationService
    {
        readonly IEventStore _eventStore;

        public FoodApplicationService(IEventStore eventStore)
        {
            Ensure.NotNull(eventStore, "eventStore");
            this._eventStore = eventStore;
        }
        public void Update(FoodId id, Action<Food> execute)
        {
            while (true)
            {
                EventStream eventStream = _eventStore.LoadEventStream(id);
                Food food = new Food(eventStream.Events);

                execute(food);

                try
                {
                    _eventStore.AppendToStream(id, eventStream.Version, food.Changes);
                    return;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    foreach (var facetEvent in food.Changes)
                    {
                        foreach (var foodEvent in ex.ActualEvents)
                        {
                            if (ConflictsWith(facetEvent, foodEvent))
                            {
                                var msg = string.Format("Conflict between {0} and {1}", facetEvent, foodEvent);
                                throw new RealConcurrencyException(msg, ex);
                            }
                        }
                    }
                    _eventStore.AppendToStream(id, ex.ActualVersion, food.Changes);
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
        public void When(AddNewFood c)
        {
            Update(c.Id, a => a.AddNewFood(c.Id, c.Category, c.Name, c.FoodTerms));
        }
        public void When(AddNewFoodTermToFood c)
        {
            Update(c.Id, a => a.AddNewFoodTerm(c.Id, c.FoodTerm));
        }
    }
}
