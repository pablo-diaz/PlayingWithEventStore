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
            int currentVersion, string serializedData, CancellationToken cancellationToken)
        {
            await _eventStoreClient.AppendToStreamAsync(streamName, StreamState.Any, new[] {
                new EventData(Uuid.NewUuid(), eventType, JsonSerializer.SerializeToUtf8Bytes(serializedData))
            }, cancellationToken: cancellationToken);
        }

        protected override async Task<List<(string eventType, string serializedData)>> LoadEventsAsync(
            string fromStreamName, CancellationToken cancellationToken)
        {
            var results = new List<(string eventType, string serializedData)>();

            try
            {
                var streamResult = _eventStoreClient.ReadStreamAsync(Direction.Forwards, fromStreamName,
                    StreamPosition.Start, cancellationToken: cancellationToken);

                await foreach(var result in streamResult)
                {
                    results.Add((
                        eventType: result.Event.EventType,
                        serializedData: JsonSerializer.Deserialize<string>(result.Event.Data.ToArray())
                    ));
                }

                return results;
            }
            catch (StreamNotFoundException)
            {
                return results;
            }
        }
    }
}
