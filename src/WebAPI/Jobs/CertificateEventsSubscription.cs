using System;
using System.Threading;
using System.Threading.Tasks;

using Domain;

using WebAPI.Jobs.Common;

using Application.Utils;
using Application.Commands.StoreEvents;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Jobs
{
    public class CertificateEventsSubscription : BackgroundService
    {
        private readonly ILogger<CertificateEventsSubscription> _logger;
        private readonly IServiceProvider _services;

        public CertificateEventsSubscription(ILogger<CertificateEventsSubscription> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("******* Starting CertificateEventsSubscription *******");

            using (var scope = this._services.CreateScope())
            {
                return scope.ServiceProvider
                    .GetRequiredService<EventsStore>()
                    .SubscribeToEventsAsync<Certificate>(ProcessEvent, stoppingToken);
            }
        }
        
        private Task ProcessEvent(EventInformation<Certificate> eventInfo, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"\tNew Certificate event received: {eventInfo.Type}");

            if (eventInfo.MaybeParsedEvent.HasNoValue)
            {
                _logger.LogInformation($"Event {eventInfo.OriginalId} could not be parsed");
                return Task.CompletedTask;
            }

            return eventInfo.MaybeParsedEvent.Value.Data switch {
                NewCertificateHasBeenRegistered @new => ProcessEvent(eventInfo.MaybeParsedEvent.Value.Id, @new),
                CertificateHasBeenSigned signed => ProcessEvent(eventInfo.MaybeParsedEvent.Value.Id, signed),
                _ => LogNotExistingHandlerForEvent(eventInfo.Type, eventInfo.OriginalId)
            };
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
