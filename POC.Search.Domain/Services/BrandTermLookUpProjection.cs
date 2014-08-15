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
    public class BrandTermLookUpProjection : IProjection
    {
        readonly IDocumentWriter<unit, BrandTermLookUp> _writer;
        public BrandTermLookUpProjection(IDocumentWriter<unit,BrandTermLookUp> writer)
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
            Action<BrandTermLookUp>[] commands =
                events.Select(e => (Action<BrandTermLookUp>)((dynamic)this).CreateCommand((dynamic)e)).
                ToArray();

            _writer.AddOrUpdate(key, commands);
        }

        public void When(BrandAdded e)
        {
            var key = new unit();
            _writer.UpdateEnforcingNew(key, CreateCommand(e));
        }
        private Action<BrandTermLookUp> CreateCommand(BrandAdded e)
        {
            Action<BrandTermLookUp> update = (tl) =>
                {
                    var newItems =
                        e.
                        BrandTerms.
                        Select(t => new BrandTermLookUpValue(t.Term, t.BrandId)).
                        ToDictionary(keySelector: (key) => key.Term,
                        comparer: StringComparer.InvariantCultureIgnoreCase,
                        elementSelector: (element) => new BrandTermLookUpValue[] { new BrandTermLookUpValue(element.Term, element.BrandId) });

                    newItems.
                        Keys.
                        ToList().
                        ForEach(key =>
                        {
                            if (!tl.Brands.ContainsKey(key))
                            {
                                tl.Brands.Add(key, newItems[key].ToList());
                            }
                            else
                            {
                                var currentList = tl.Brands[key];
                                var newList = newItems[key].
                                                Where(item => !currentList.Contains(item));
                                tl.Brands[key] = currentList.Concat(newList).ToList();

                            }
                        }
                    );
                };
            return update;
        }
        public void When(TermAddedToBrand e)
        {
            var key = new unit();
            _writer.UpdateOrThrow(key, CreateCommand(e));
        }
        private Action<BrandTermLookUp> CreateCommand(TermAddedToBrand e)
        {
            Action<BrandTermLookUp> update = (tl) =>
            {
                var newValue = new BrandTermLookUpValue(e.BrandTerm.Term, e.Id);
                var key = e.BrandTerm.Term;

                if (!tl.Brands.ContainsKey(key))
                {
                    var terms = new BrandTermLookUpValue[] { newValue };
                    tl.Brands.Add(key, terms.ToList());
                }
                else
                {

                    if (!tl.Brands[key].Contains(newValue)) tl.Brands[key].Add(newValue);
                }
            };

            return update;
        }
    }
}
