using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Aggregates
{
    public class CategoryState
    {
        public CategoryState(IEnumerable<IEvent> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        public void Mutate(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
        }

        public void When(NewCategoryAdded e)
        {
            this.Id = e.Id;
            this.Name = e.Name;
            this.Aliases = e.Aliases;
            this.Parent = e.Parent;
        }
        public void When(AddAliasToCategory e)
        {
            List<CategoryAlias> items = new List<CategoryAlias>();
            if (this.Aliases != null) items.AddRange(this.Aliases);
            items.Add(e.Alias);
            this.Aliases = items.ToArray();
        }
        
        public CategoryId Id { get; private set; }
        public string Name { get; private set; }
        public CategoryId Parent { get; private set; }
        public CategoryAlias[] Aliases { get; private set; }
    }
}
