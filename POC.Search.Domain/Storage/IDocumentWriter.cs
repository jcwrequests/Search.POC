using POC.Search.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public interface IDocumentWriter<in TKey, TEntity>
    {
        TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
        bool TryDelete(TKey key);
        TEntity AddOrUpdate(TKey key, Action<TEntity>[] updates, AddOrUpdateHint hint = AddOrUpdateHint.ProbablyExists);
    }
}
