using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class Food
    {
        public readonly IList<IEvent> Changes = new List<IEvent>();
        readonly FoodState _state;

        public Food(IEnumerable<IEvent> events)
        {
            this._state = new FoodState(events);

        }
        public void Apply(IEvent e)
        {
            _state.Mutate(e);
            Changes.Add(e);
        }
        public void AddNewFood(FoodId id, CategoryId category, string name, params FoodTerm[] foodTerms)
        {
            if (_state.Id != null) throw new Exception("Food already created");
            Apply(new NewFoodAdded(category, id, name, foodTerms));
        }
        public void AddNewFoodTerm(FoodId id, FoodTerm foodTerm)
        {
            if (_state.Id != id) throw new ArgumentException("Invalid Id");
            List<FoodTerm> terms = new List<FoodTerm>();
            if (terms.Contains(foodTerm)) throw new ArgumentException("Food Term Already Exists");
             Apply(new NewFoodTermAddedToFood(id,foodTerm));
        }
        
    }
}
