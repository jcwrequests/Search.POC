using POC.Search.Domain.Services;
using POC.Search.Domain.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POC.Search.Domain.Services
{
    public delegate void UpdateIndexHandler(long value);

    public class DomainEventPublisher
    {
        private Dictionary<Type, List<object>> projectionsByEventType;
        private Dictionary<IProjection, List<Type>> projections;
        private IEventStore store;
        private long currentIndexValue;
        public event UpdateIndexHandler UpdateIndex;
        private long totalNewEvents = 0;
        private ILogger log;
        private int numberOfEventsForRebuild;

        public DomainEventPublisher(IEventStore store,
                                    long currentIndexValue,
                                    ILogger log,
                                    int numberOfEventsForRebuild = 5000)
        {
            if (store == null) throw new ArgumentNullException("store");
            if (log == null) throw new ArgumentNullException("log");

            this.store = store;
            this.log = log;
            this.numberOfEventsForRebuild = numberOfEventsForRebuild;
            projectionsByEventType = new Dictionary<Type, List<object>>();
            projections = new Dictionary<IProjection, List<Type>>();
            this.currentIndexValue = currentIndexValue;

        }

        public void Start()
        {
            store.NewEventsArrived += OnNewIndex;

            if (IsForceRebuild() || IsOutSyncWithStore())
            {
                if (this.currentIndexValue < 0) currentIndexValue *= -1;
                log.Info("Rebuild Projections");
                Rebuild();
                this.currentIndexValue = store.GetCurrentVersion();
                UpdateIndex(this.currentIndexValue);
            }

            if (projectionsByEventType.Count.Equals(0)) throw new InvalidOperationException("Must supply at least one projection");


        }

        private bool IsForceRebuild()
        {
            return this.currentIndexValue <= 0;
        }
        private bool IsOutSyncWithStore()
        {
            long currentStoreVersion = store.GetCurrentVersion();
            return currentStoreVersion > this.currentIndexValue;
        }
        public void StopProcessingEvents()
        {
            store.NewEventsArrived -= OnNewIndex;
        }

        public void RegisterProjection<TProjection>(TProjection projection) where TProjection : class, IProjection
        {
            var eventTypes = projection.
                             GetType().
                             GetMethods().
                             Where(m => m.Name.Equals("when", StringComparison.InvariantCultureIgnoreCase)).
                             Where(m => m.GetParameters().Count().Equals(1)).
                             Select(m => m.GetParameters().First().ParameterType);


            foreach (var eventType in eventTypes)
            {
                if (!projectionsByEventType.ContainsKey(eventType))
                {
                    projectionsByEventType.Add(eventType, new List<object>());
                }
                projectionsByEventType[eventType].Add(projection);

            }
            if (!projections.ContainsKey(projection)) projections.Add(projection, eventTypes.ToList());

        }

        public void RegisterProjections<TProjection>(params TProjection[] projections) where TProjection : class, IProjection
        {
            foreach (IProjection projection in projections)
            {
                RegisterProjection(projection);
            }
        }

        private void Rebuild()
        {

            while (true)
            {
                int count = numberOfEventsForRebuild;
                long indexValue = System.Threading.Interlocked.Read(ref currentIndexValue);
                var newEvents = store.LoadEventStream(indexValue, (int)count);
                var newEventsCount = newEvents.Count;

                if (!newEventsCount.Equals(0))
                {
                    projections.
                        Keys.
                        ToList().
                        ForEach(k =>
                        {
                            var types = projections[k];
                            var events = newEvents.
                                Where(e => types.Contains(e.GetType()));
                            k.Execute(events.ToArray());
                        });
                    if (UpdateIndex != null) UpdateIndex(newEventsCount);
                    System.Threading.Interlocked.Add(ref currentIndexValue, newEventsCount);

                    if (newEventsCount < count)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }


        }
        private void OnNewIndex(long count)
        {
            log.Debug("OnNewIndex");
            bool indexChanged = false;
            long indexValue = System.Threading.Interlocked.Read(ref currentIndexValue);

            log.Debug(count.ToString());
            log.Debug(indexValue.ToString());
            log.Debug(store.GetCurrentVersion().ToString());

            var newEvents = store.LoadEventStream(indexValue, (int)count);
            if (!newEvents.Count.Equals(0))
            {
                System.Threading.Interlocked.Add(ref currentIndexValue, newEvents.Count());
                indexChanged = true;
            }

            int newEventCount = newEvents.Count();
            log.Debug(newEventCount.ToString());

            foreach (IEvent @event in newEvents)
            {
                if (projectionsByEventType.ContainsKey(@event.GetType()))
                {
                    var projections = projectionsByEventType[@event.GetType()];
                    foreach (IProjection projection in projections)
                    {
                        try
                        {
                            projection.Execute(@event);
                        }
                        catch (Exception ex)
                        {
                            var reason = string.
                                        Format("Failed to execute projection for {0} and event type {1} reason:  {2}",
                                                projection.GetType().ToString(),
                                                @event.GetType().ToString(),
                                                ex.ToString());

                            log.Error(reason);
                            log.System(@event, projection.GetType(), reason, SystemEventTypes.Error);
                        }

                    }
                }
                else
                {
                    log.Info(string.Format("event type {0} has no projections", @event.GetType().ToString()));
                }
            }

            if (indexChanged && UpdateIndex != null)
            {
                UpdateIndex(indexValue);
            }
        }


    }
}
