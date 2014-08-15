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
    public class FoodLookUpProjection : IProjection
    {
        readonly IDocumentWriter<unit, FoodLookUp> _writer; 

        public FoodLookUpProjection(IDocumentWriter<unit,FoodLookUp> writer)
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
            Action<FoodLookUp>[] commands =
                events.Select(e => (Action<FoodLookUp>)((dynamic)this).CreateCommand((dynamic)e)).
                ToArray();

            _writer.AddOrUpdate(key, commands);
        }

        public void When(NewFoodAdded e)
        {
            var key = new unit();
            _writer.UpdateEnforcingNew(key, CreateCommand(e));
        }

        private Action<FoodLookUp> CreateCommand(NewFoodAdded e)
        {
            Action<FoodLookUp> update = (cat) =>
            {
                FoodLookUpValue category = new FoodLookUpValue(e.Category,e.Id,e.Name,e.FoodTerms);
                if (!cat.Foods.ContainsKey(e.Id)) cat.Foods.Add(e.Id, category);
            };
            return update;
        }
    }
}
