using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        protected abstract Task SubscribeToEventsAsync(string eventPrefix,
            Action<string, string, Maybe<string>> callbackFn,
            CancellationToken cancellationToken);

        public Task SubscribeToEventsAsync<T>(Func<EventInformation<T>, CancellationToken, Task> callbackFn,
                CancellationToken cancellationToken) where T : AggregateRoot =>
            SubscribeToEventsAsync(Utilities.GetStreamPrefix<T>(),
                async (originalId, eventType, maybeSerializedData) => {
                    await callbackFn(new EventInformation<T>(originalId, eventType, maybeSerializedData), cancellationToken);
            }, cancellationToken);

        private string BuildStreamName<T>(Guid forId) where T : AggregateRoot =>
            $"{Utilities.GetStreamPrefix<T>()}{forId}";
    }
}
