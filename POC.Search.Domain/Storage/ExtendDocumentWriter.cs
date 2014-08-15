using POC.Search.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public static class ExtendDocumentWriter
    {
        public static TEntity AddOrUpdate<TKey, TEntity>(this IDocumentWriter<TKey, TEntity> self, TKey key, Func<TEntity> addFactory, Action<TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
        {
            return self.AddOrUpdate(key, addFactory, entity =>
            {
                update(entity);
                return entity;
            }, hint);
        }
        public static TEntity AddOrUpdate<TKey, TEntity>(this IDocumentWriter<TKey, TEntity> self, TKey key, TEntity newView, Action<TEntity> updateViewFactory, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
        {
            return self.AddOrUpdate(key, () => newView, view =>
            {
                updateViewFactory(view);
                return view;
            }, hint);
        }
        public static TEntity Add<TKey, TEntity>(this IDocumentWriter<TKey, TEntity> self, TKey key, TEntity newEntity)
        {
            return self.AddOrUpdate(key, newEntity, e =>
            {
                var txt = String.Format("Entity '{0}' with key '{1}' should not exist.", typeof(TEntity).Name, key);
                throw new InvalidOperationException(txt);
            }, AddOrUpdateHint.ProbablyDoesNotExist);
        }
        public static TEntity UpdateOrThrow<TKey, TEntity>(this IDocumentWriter<TKey, TEntity> self, TKey key, Func<TEntity, TEntity> change)
        {
            return self.AddOrUpdate(key, () =>
            {
                var txt = String.Format("Failed to load '{0}' with key '{1}'.", typeof(TEntity).Name, key);
                throw new InvalidOperationException(txt);
            }, change, AddOrUpdateHint.ProbablyExists);
        }
        public static TEntity UpdateOrThrow<TKey, TEntity>(this IDocumentWriter<TKey, TEntity> self, TKey key, Action<TEntity> change)
        {
            return self.AddOrUpdate(key, () =>
            {
                var txt = String.Format("Failed to load '{0}' with key '{1}'.", typeof(TEntity).Name, key);
                throw new InvalidOperationException(txt);
            }, change, AddOrUpdateHint.ProbablyExists);
        }
        public static TView CreateView<TView>() where TView : new()
        {
            return new TView();
        }
        public static TView UpdateEnforcingNew<TKey, TView>(this IDocumentWriter<TKey, TView> self, TKey key,
            Action<TView> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
            where TView : new()
        {
            return self.AddOrUpdate(key, () =>
            {
                var view = new TView();
                update(view);
                return view;
            }, v =>
            {
                update(v);
                return v;
            }, hint);
        }

        public static TView UpdateEnforcingNew<TView>(this IDocumentWriter<unit, TView> self, Action<TView> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists)
            where TView : new()
        {
            return self.UpdateEnforcingNew(unit.it, update, hint);

        }

    }
}
