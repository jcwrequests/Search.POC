using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public interface IDocumentStrategy
    {
        string GetEntityBucket<TEntity>();
        string GetEntityLocation<TEntity>(object key);


        void Serialize<TEntity>(TEntity entity, Stream stream);
        TEntity Deserialize<TEntity>(Stream stream);
    }
}
