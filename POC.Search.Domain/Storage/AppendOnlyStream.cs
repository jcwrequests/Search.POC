using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace POC.Search.Domain.Storage
{
    public sealed class AppendOnlyStream : IDisposable
    {
        readonly int _pageSizeInBytes;
        readonly AppendWriterDelegate _writer;
        readonly int _maxByteCount;
        MemoryStream _pending;

        int _bytesWritten;
        int _bytesPending;
        int _fullPagesFlushed;

        public AppendOnlyStream(int pageSizeInBytes, AppendWriterDelegate writer, int maxByteCount)
        {
            _writer = writer;
            _maxByteCount = maxByteCount;
            _pageSizeInBytes = pageSizeInBytes;
            _pending = new MemoryStream();
        }

        public bool Fits(int byteCount)
        {
            return (_bytesWritten + byteCount <= _maxByteCount);
        }

        public void Write(byte[] buffer)
        {
            _pending.Write(buffer, 0, buffer.Length);
            _bytesWritten += buffer.Length;
            _bytesPending += buffer.Length;
        }

        public void Flush()
        {
            if (_bytesPending == 0)
                return;

            var size = (int)_pending.Length;
            var padSize = (_pageSizeInBytes - size % _pageSizeInBytes) % _pageSizeInBytes;

            using (var stream = new MemoryStream(size + padSize))
            {
                stream.Write(_pending.ToArray(), 0, (int)_pending.Length);
                if (padSize > 0)
                    stream.Write(new byte[padSize], 0, padSize);

                stream.Position = 0;
                _writer(_fullPagesFlushed * _pageSizeInBytes, stream);
            }

            var fullPagesFlushed = size / _pageSizeInBytes;

            if (fullPagesFlushed <= 0)
                return;

            var newStream = new MemoryStream();
            _pending.Position = fullPagesFlushed * _pageSizeInBytes;
            _pending.CopyTo(newStream);
            _pending.Dispose();
            _pending = newStream;

            _fullPagesFlushed += fullPagesFlushed;
            _bytesPending = 0;
        }

        public void Dispose()
        {
            Flush();
            _pending.Dispose();
        }
    }

    public delegate void AppendWriterDelegate(int offset, Stream source);
}
