using POC.Search.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public sealed class FileDocumentReaderWriter<TKey, TEntity> : IDocumentReader<TKey, TEntity>,
                                                                  IDocumentWriter<TKey, TEntity>
    {
        readonly IDocumentStrategy _strategy;
        readonly string _folder;
        TEntity _cache;
        object cacheLock;

        public FileDocumentReaderWriter(string fileDocumentReaderWriterDirectoryPath, IDocumentStrategy strategy)
        {
            _strategy = strategy;
            _folder = Path.Combine(fileDocumentReaderWriterDirectoryPath, strategy.GetEntityBucket<TEntity>());
            cacheLock = new object();
        }

        public void InitIfNeeded()
        {
            Directory.CreateDirectory(_folder);
        }

        public bool TryGet(TKey key, out TEntity view)
        {
            if (_cache != null)
            {
                view = _cache;
                return true;
            }

            view = default(TEntity);
            try
            {
                var name = GetName(key);

                if (!File.Exists(name))
                    return false;

                using (var stream = File.Open(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (stream.Length == 0)
                        return false;
                    view = _strategy.Deserialize<TEntity>(stream);
                    lock (cacheLock)
                    {
                        _cache = view;
                    }

                    return true;
                }
            }
            catch (FileNotFoundException)
            {
                // if file happened to be deleted between the moment of check and actual read.
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }
        }

        string GetName(TKey key)
        {
            return Path.Combine(_folder, _strategy.GetEntityLocation<TEntity>(key));
        }
        public TEntity AddOrUpdate(TKey key, Action<TEntity>[] updates, AddOrUpdateHint hint)
        {
            var name = GetName(key);
            TEntity view;

            try
            {

                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);

                using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] initial = new byte[0];

                    if (file.Length == 0)
                    {
                        view = Activator.CreateInstance<TEntity>();
                    }
                    else
                    {
                        using (var mem = new MemoryStream())
                        {
                            file.CopyTo(mem);
                            mem.Seek(0, SeekOrigin.Begin);
                            view = _strategy.Deserialize<TEntity>(mem);
                        }
                    }

                    lock (cacheLock)
                    {
                        updates.
                            ToList().
                            ForEach(update =>
                            {
                                update(view);
                                if (_cache != null) update(_cache);
                                if (_cache == null) _cache = view;
                            }
                            );
                    };


                    // some serializers have nasty habbit of closing the
                    // underling stream
                    using (var mem = new MemoryStream())
                    {
                        _strategy.Serialize(view, mem);
                        var data = mem.ToArray();

                        if (!data.SequenceEqual(initial))
                        {
                            // upload only if we changed
                            file.Seek(0, SeekOrigin.Begin);
                            file.Write(data, 0, data.Length);
                            // truncate this file
                            file.SetLength(data.Length);
                        }
                    }

                    return view;
                }
            }
            catch (DirectoryNotFoundException)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }
        public TEntity AddOrUpdate(TKey key, Func<TEntity> addFactory, Func<TEntity, TEntity> update,
            AddOrUpdateHint hint)
        {
            var name = GetName(key);

            try
            {
                // This is fast and allows to have git-style subfolders in atomic strategy
                // to avoid NTFS performance degradation (when there are more than 
                // 10000 files per folder). Kudos to Gabriel Schenker for pointing this out
                var subfolder = Path.GetDirectoryName(name);
                if (subfolder != null && !Directory.Exists(subfolder))
                    Directory.CreateDirectory(subfolder);


                // we are locking this file.
                // original using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                // changed the file share access to read to allow for autobackup to work
                using (var file = File.Open(name, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    byte[] initial = new byte[0];
                    TEntity result;
                    if (file.Length == 0)
                    {
                        result = addFactory();
                    }
                    else
                    {
                        using (var mem = new MemoryStream())
                        {
                            file.CopyTo(mem);
                            mem.Seek(0, SeekOrigin.Begin);
                            var entity = _strategy.Deserialize<TEntity>(mem);
                            initial = mem.ToArray();
                            result = update(entity);
                            lock (cacheLock)
                            {
                                _cache = update(entity);
                            }

                        }
                    }

                    // some serializers have nasty habbit of closing the
                    // underling stream
                    using (var mem = new MemoryStream())
                    {
                        _strategy.Serialize(result, mem);
                        var data = mem.ToArray();

                        if (!data.SequenceEqual(initial))
                        {
                            // upload only if we changed
                            file.Seek(0, SeekOrigin.Begin);
                            file.Write(data, 0, data.Length);
                            // truncate this file
                            file.SetLength(data.Length);
                        }
                    }

                    return result;
                }
            }
            catch (DirectoryNotFoundException)
            {
                var s = string.Format(
                    "Container '{0}' does not exist.",
                    _folder);
                throw new InvalidOperationException(s);
            }
        }

        public bool TryDelete(TKey key)
        {
            var name = GetName(key);
            if (File.Exists(name))
            {
                File.Delete(name);
                return true;
            }
            return false;
        }
    }
}
