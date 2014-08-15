using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class CategoryLookUpValue
    {
        public CategoryLookUpValue(CategoryId id,
                                   string name,
                                   CategoryId parent,
                                   params CategoryAlias[] categoryAliases)
        {
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            this.Id = id;
            this.Name = name;
            this.Parent = parent;
            this.CategoryAliases = categoryAliases;
        }
        public CategoryId Id { get; private set; }
        public string Name { get; private set; }
        public CategoryId Parent { get; private set; }
        public CategoryAlias[] CategoryAliases { get; private set; } 
    }
}
