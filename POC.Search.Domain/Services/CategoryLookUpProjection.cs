using POC.Search.Domain.Contracts;
using POC.Search.Domain.Infrastructure;
using POC.Search.Domain.Storage;
using POC.Search.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace POC.Search.Domain.Services
{
    public class CategoryLookUpProjection : IProjection
    {
        readonly IDocumentWriter<unit, CategoryLookUp> _writer; 

        public CategoryLookUpProjection(IDocumentWriter<unit,CategoryLookUp> writer)
        {
            Ensure.NotNull(writer, "writer");
            this._writer = writer;
        }
        public void Execute(IEvent e)
        {
            ((dynamic)this).When((dynamic)e);
        }

        public void Execute(IEvent[] events)
        {
            var key = new unit();
            Action<CategoryLookUp>[] commands =
                events.Select(e => (Action<CategoryLookUp>)((dynamic)this).CreateCommand((dynamic)e)).
                ToArray();

            _writer.AddOrUpdate(key, commands);
        }

        public void When(NewCategoryAdded e)
        {
            var key = new unit();
            _writer.UpdateEnforcingNew(key, CreateCommand(e));
        }

        private Action<CategoryLookUp> CreateCommand(NewCategoryAdded e)
        {
            Action<CategoryLookUp> update = (cat) =>
            {
                CategoryLookUpValue category = new CategoryLookUpValue(e.Id, e.Name, e.Parent, e.Aliases);
                if (!cat.Categories.ContainsKey(e.Id)) cat.Categories.Add(e.Id,category);
            };
            return update;
        }
    }
}
