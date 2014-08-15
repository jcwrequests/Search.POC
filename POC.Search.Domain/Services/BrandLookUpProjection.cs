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
    public class BrandLookUpProjection : IProjection
    {
        readonly IDocumentWriter<brandLookUpunit, BrandLookUp> _writer;

        public BrandLookUpProjection(IDocumentWriter<brandLookUpunit, BrandLookUp> writer)
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
            var key = new brandLookUpunit();
            Action<BrandLookUp>[] commands =
                events.Select(e => (Action<BrandLookUp>)((dynamic)this).CreateCommand((dynamic)e)).
                ToArray();

            _writer.AddOrUpdate(key, commands);
        }
        public void When(BrandAdded e)
        {
            var key = new brandLookUpunit();
            _writer.UpdateEnforcingNew(key, CreateCommand(e));
        }

        private Action<BrandLookUp> CreateCommand(BrandAdded e)
        {
            Action<BrandLookUp> update = (bl) =>
                {
                    BrandLookUpValue brand = new BrandLookUpValue(e.Id, e.Name,e.BrandTerms);
                    if (!bl.Brands.ContainsKey(e.Id)) bl.Brands.Add(e.Id,brand);
                };
            return update;
        }
    }
}
