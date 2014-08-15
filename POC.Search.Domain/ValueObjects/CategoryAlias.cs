using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
    [Serializable]
    public sealed class CategoryAlias
    {
        public CategoryAlias(CategoryId categoryId, string alias)
        {
            Ensure.NotNull(categoryId, "categoryId");
            Ensure.NotNull(alias, "alias");
        }
       public CategoryId CategoryId {get; private set;}
       public String Alias {get; private set;}
    }
}
