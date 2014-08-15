using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public class FoodLookUpValue
    {
        public FoodLookUpValue(CategoryId category, FoodId id, string name, params FoodTerm[] foodTerms)
        {
            Ensure.NotNull(category, "category");
            Ensure.NotNull(id, "id");
            Ensure.NotNull(name, "name");
            this.Category = category;
            this.Id = id;
            this.Name = name;
            this.FoodTerms = foodTerms;
        }

        public CategoryId Category { get; private set; }
        public FoodId Id { get; private set; }
        public string Name { get; private set; }
        public FoodTerm[] FoodTerms { get; private set; }
    }
}
