using System;
using System.Threading;
using System.Threading.Tasks;

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
                var docStore = scope.ServiceProvider.GetRequiredService<DocumentsStore>();

                return scope.ServiceProvider
                    .GetRequiredService<EventsStore>()
                    .SubscribeToEventsAsync<Domain.Certificate>((ei, ct) =>
                        ProcessEvent(docStore, ei, ct), stoppingToken);
            }
        }
        
        private Task ProcessEvent(DocumentsStore withDocStore,
            EventInformation<Domain.Certificate> eventInfo, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"\tNew Certificate event received: {eventInfo.Type}");

            if (eventInfo.MaybeParsedEvent.HasNoValue)
            {
                _logger.LogInformation($"Event {eventInfo.OriginalId} could not be parsed");
                return Task.CompletedTask;
            }

            return eventInfo.MaybeParsedEvent.Value.Data switch {
                NewCertificateHasBeenRegistered @new =>
                    ProcessEvent(withDocStore, eventInfo.MaybeParsedEvent.Value.Id, @new, cancellationToken),
                CertificateHasBeenSigned signed =>
                    ProcessEvent(withDocStore, eventInfo.MaybeParsedEvent.Value.Id, signed, cancellationToken),
                _ => LogNotExistingHandlerForEvent(eventInfo.Type, eventInfo.OriginalId)
            };
        }

        private Task ProcessEvent(DocumentsStore withDocStore, Guid withId,
            NewCertificateHasBeenRegistered @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing NewCertificateHasBeenRegistered event");

            var doc = new Application.Queries.Documents.Certificate {
                Id = withId.ToString(),
                Number = @event.Number,
                Status = "Draft"
            };

            return withDocStore.SaveAsync(doc, cancellationToken);
        }

        private Task ProcessEvent(DocumentsStore withDocStore, Guid withId,
            CertificateHasBeenSigned @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing CertificateHasBeenSigned event");

            var doc = new Application.Queries.Documents.Certificate
            {
                Id = withId.ToString(),
                Status = "Signed",
                SignedAt = @event.SignedAt.ToString(),
                SignedBy = @event.SignedBy
            };

            return withDocStore.SaveAsync(doc, cancellationToken);
        }

        private Task LogNotExistingHandlerForEvent(string forEventType, string forEventId)
        {
            _logger.LogInformation($"Event handler for {forEventType} has not been implemented yet; event id is: {forEventId}");

            return Task.CompletedTask;
        }
    }
}
