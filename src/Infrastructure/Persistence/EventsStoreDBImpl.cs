using System;
using System.Threading;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

using Application.Utils;

using EventStore.Client;


namespace Infrastructure.Persistence
{
    public sealed class EventsStoreDBImpl : EventsStore
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventsStoreDBImpl(EventStoreClient eventStoreClient)
        {
            this._eventStoreClient = eventStoreClient;
        }

        protected override async Task SaveAsync(string streamName, string eventType,
            int currentVersion, string serializedEventData, CancellationToken cancellationToken)
        {
            await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, new[] {
                new EventData(Uuid.NewUuid(), eventType, JsonSerializer.SerializeToUtf8Bytes(serializedEventData))
            }, cancellationToken: cancellationToken);
        }

        protected override Task<IEnumerable<string>> LoadEventsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
