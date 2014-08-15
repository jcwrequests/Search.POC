using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using POC.Search.Domain.Exceptions;

namespace POC.Search.Domain.Storage
{
    public class EventStore : IEventStore
    {

        readonly BinaryFormatter _formatter = new BinaryFormatter();

        byte[] SerializeEvent(IEvent[] e)
        {
            using (var mem = new MemoryStream())
            {
                _formatter.Serialize(mem, e);
                return mem.ToArray();
            }
        }

        IEvent[] DeserializeEvent(byte[] data)
        {
            using (var mem = new MemoryStream(data))
            {
                return (IEvent[])_formatter.Deserialize(mem);
            }
        }

        public EventStore(IAppendOnlyStore appendOnlyStore)
        {
            _appendOnlyStore = appendOnlyStore;
        }

        readonly IAppendOnlyStore _appendOnlyStore;

        public long GetCurrentVersion()
        {
            return this._appendOnlyStore.GetCurrentVersion();
        }

        public EventStream LoadEventStream(IIdentity id, long skip, int take)
        {
            var name = id.ToString();
            var records = _appendOnlyStore.ReadRecords(name, skip, take).ToList();
            var stream = new EventStream();

            foreach (var tapeRecord in records)
            {
                stream.Events.AddRange(DeserializeEvent(tapeRecord.Data));
                stream.Version = tapeRecord.Version;
            }
            return stream;
        }

        public EventStream LoadEventStream(IIdentity id)
        {
            return LoadEventStream(id, 0, int.MaxValue);
        }

        public List<IEvent> LoadEventStream(long skip, int take)
        {
            var records = _appendOnlyStore.ReadRecords(skip, take);
            var events = new List<IEvent>();

            foreach (var record in records)
            {
                events.AddRange(DeserializeEvent(record.Data));
            }
            return events;
        }

        public void AppendToStream(IIdentity id, long originalVersion, ICollection<IEvent> events)
        {
            if (events.Count == 0)
                return;
            var name = id.ToString();
            var data = SerializeEvent(events.ToArray());
            try
            {
                _appendOnlyStore.Append(name, data, originalVersion);
                if (NewEventsArrived != null) NewEventsArrived(events.Count());

            }
            catch (AppendOnlyStoreConcurrencyException e)
            {
                // load server events
                var server = LoadEventStream(id, 0, int.MaxValue);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(server.Version, e.ExpectedStreamVersion, id.ToString(), server.Events);
            }

        }


        public event NewEventsArrivedHandler NewEventsArrived;



    }
}
