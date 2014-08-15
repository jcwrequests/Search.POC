using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public interface IAppendOnlyStore : IDisposable
    {
        void Append(string streamName, byte[] data, long expectedStreamVersion = -1);
        IEnumerable<DataWithVersion> ReadRecords(string streamName, long afterVersion, int maxCount);
        IEnumerable<DataWithName> ReadRecords(long afterVersion, int maxCount);
        long GetCurrentVersion();
        void Close();
    }

    public sealed class DataWithVersion
    {
        public readonly long Version;
        public readonly byte[] Data;

        public DataWithVersion(long version, byte[] data)
        {
            Version = version;
            Data = data;
        }
    }
    public sealed class DataWithName
    {
        public readonly string Name;
        public readonly byte[] Data;

        public DataWithName(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }


}
