using POC.Search.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.ValueObjects
{
   [Serializable]
   public class Facet
    {
       public Facet(FacetId id, string name)
       {
           Ensure.NotNull(id, "id");
           Ensure.NotNull(name, "name");
           this.Id = id;
           this.Name = name;
       }
       public FacetId Id { get; private set; }
       public string Name { get; private set; }
    }
}
