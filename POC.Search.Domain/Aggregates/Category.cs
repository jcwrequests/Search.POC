using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class Category
    {
        public Category(IEnumerable<IEvent> events)
        {
            this._state = new CategoryState(events);
        }
        public readonly IList<IEvent> Changes = new List<IEvent>();
        readonly CategoryState _state;

        void Apply(IEvent e)
        {
            _state.Mutate(e);
            Changes.Add(e);
        }

        public void AddNewCategory(CategoryId id, string name,CategoryId parent, CategoryAlias[] aliases)
        {
            if (_state.Id != null) throw new Exception("This Category has already been created.");
            Apply(new NewCategoryAdded(id, name,parent, aliases));
        }
        public void AddNewAlias(CategoryId id, CategoryAlias alias)
        {
            if (_state.Id == null) throw new Exception("You can not add an alias before a category is created");
            if (_state.Id != id) throw new Exception("Invalid Id");
           Apply(new AliasAddedToCategory(id,alias));
        }
    }
}
