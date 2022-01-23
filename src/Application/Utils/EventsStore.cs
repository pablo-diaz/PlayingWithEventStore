using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Domain;
using Domain.Common;

using Application.Commands.StoreEvents;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public abstract class EventsStore
    {
        internal Task AppendEvent<T>(T aggregateToAppend, EventInStore @event,
                CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot =>
            SaveAsync(streamName: BuildStreamName<T>(aggregateToAppend.Id), @event.ApplicationEventType,
                currentVersion: 1, @event.Serialize(), cancellationToken);

        internal async Task<Maybe<T>> Load<T>(Guid byId,
                Func<List<(string eventType, string serializedData)>, Maybe<T>> eventsApplierFn,
                CancellationToken cancellationToken = new CancellationToken()) where T: AggregateRoot =>
            eventsApplierFn(await LoadEventsAsync(fromStreamName: BuildStreamName<T>(byId), cancellationToken));

        protected abstract Task SaveAsync(string streamName, string eventType,
            int currentVersion, string serializedData, CancellationToken cancellationToken);

        protected abstract Task<List<(string eventType, string serializedData)>> LoadEventsAsync(
            string fromStreamName, CancellationToken cancellationToken);

        private string BuildStreamName<T>(Guid forId) where T : AggregateRoot
        {
            var requestedType = typeof(T);
            if (requestedType == typeof(Certificate)) return $"Certificate-{forId}";

            throw new NotImplementedException("Unknown Aggregate");
        }
    }
}
