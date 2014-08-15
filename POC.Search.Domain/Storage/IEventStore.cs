using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Storage
{
    public delegate void NewEventsArrivedHandler(long count);

    public interface IEventStore
    {
        EventStream LoadEventStream(IIdentity id);
        EventStream LoadEventStream(IIdentity id, long skipEvents, int maxCount);
        void AppendToStream(IIdentity id, long expectedVersion, ICollection<IEvent> events);
        List<IEvent> LoadEventStream(long skipEvents, int maxCount);
        event NewEventsArrivedHandler NewEventsArrived;
        long GetCurrentVersion();
    }
}
