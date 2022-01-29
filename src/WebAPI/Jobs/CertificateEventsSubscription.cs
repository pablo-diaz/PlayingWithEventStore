using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using WebAPI.Jobs.Common;

using Application.Commands.StoreEvents;

using EventStore.Client;

using Microsoft.Extensions.Logging;

using CSharpFunctionalExtensions;

namespace WebAPI.Jobs
{
    public class CertificateEventsSubscription : BackgroundService
    {
        private static readonly string EventPrefix = "Certificate-";

        private readonly EventStoreClient _eventStoreClient;
        private readonly ILogger<CertificateEventsSubscription> _logger;

        public CertificateEventsSubscription(EventStoreClient eventStoreClient,
            ILogger<CertificateEventsSubscription> logger)
        {
            this._eventStoreClient = eventStoreClient;
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("******* Starting CertificateEventsSubscription *******");

            await _eventStoreClient.SubscribeToAllAsync(ProcessEvent,
                filterOptions: new SubscriptionFilterOptions(StreamFilter.Prefix(EventPrefix)));
        }

        private Task ProcessEvent(StreamSubscription sub, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"\tNew Certificate event received: {resolvedEvent.Event.EventType}");

            var maybeParsedEvent = ParseEvent(resolvedEvent, resolvedEvent.Event.EventType);
            if(maybeParsedEvent.HasNoValue)
            {
                _logger.LogInformation($"Event {resolvedEvent.Event.EventStreamId} could not be parsed");
                return Task.CompletedTask;
            }

            return maybeParsedEvent.Value.@event switch {
                NewCertificateHasBeenRegistered @new => ProcessEvent(maybeParsedEvent.Value.id, @new),
                CertificateHasBeenSigned signed => ProcessEvent(maybeParsedEvent.Value.id, signed),
                _ => LogNotExistingHandlerForEvent(resolvedEvent.Event.EventType, resolvedEvent.Event.EventStreamId)
            };
        }

        private Maybe<(EventInStore @event, Guid id)> ParseEvent(ResolvedEvent fromResolvedEvent, string withEventType)
        {
            try
            {
                var id = Guid.Parse(fromResolvedEvent.Event.EventStreamId.Replace(EventPrefix, ""));
                var serializedData = JsonSerializer.Deserialize<string>(fromResolvedEvent.Event.Data.ToArray());

                if (withEventType == NewCertificateHasBeenRegistered.ApplicationEventTypeName)
                    return (EventInStore.Deserialize<NewCertificateHasBeenRegistered>(serializedData), id);

                if (withEventType == CertificateHasBeenSigned.ApplicationEventTypeName)
                    return (EventInStore.Deserialize<CertificateHasBeenSigned>(serializedData), id);

                return Maybe<(EventInStore, Guid)>.None;
            }
            catch
            {
                return Maybe<(EventInStore, Guid)>.None;
            }
        }

        private Task ProcessEvent(Guid withId, NewCertificateHasBeenRegistered @event)
        {
            _logger.LogInformation("Processing NewCertificateHasBeenRegistered event");

            return Task.CompletedTask;
        }

        private Task ProcessEvent(Guid withId, CertificateHasBeenSigned @event)
        {
            _logger.LogInformation("Processing CertificateHasBeenSigned event");

            return Task.CompletedTask;
        }

        private Task LogNotExistingHandlerForEvent(string forEventType, string forEventId)
        {
            _logger.LogInformation($"Event handler for {forEventType} has not been implemented yet; event id is: {forEventId}");
            return Task.CompletedTask;
        }
    }
}
