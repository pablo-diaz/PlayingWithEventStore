using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        private readonly Dictionary<Guid, Application.Queries.Documents.Certificate> _localCertificatesCache =
            new Dictionary<Guid, Application.Queries.Documents.Certificate>();

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
        
        private async Task ProcessEvent(DocumentsStore withDocStore,
            EventInformation<Domain.Certificate> eventInfo, CancellationToken cancellationToken)
        {
            if (eventInfo.MaybeParsedEvent.HasNoValue)
            {
                _logger.LogInformation($"Event {eventInfo.OriginalId} could not be parsed");
                return;
            }

            var task = eventInfo.MaybeParsedEvent.Value.Data switch {
                NewCertificateHasBeenRegistered @new =>
                    ProcessEvent(withDocStore, eventInfo.MaybeParsedEvent.Value.Id, @new, cancellationToken),
                CertificateHasBeenSigned signed =>
                    ProcessEvent(withDocStore, eventInfo.MaybeParsedEvent.Value.Id, signed, cancellationToken),
                _ => LogNotExistingHandlerForEvent(eventInfo.Type, eventInfo.OriginalId)
            };

            await task;
        }

        private async Task ProcessEvent(DocumentsStore withDocStore, Guid withId,
            NewCertificateHasBeenRegistered @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing NewCertificateHasBeenRegistered event");

            var existingCertificate = await GetCertificateFromCache(withId, withDocStore, cancellationToken);

            existingCertificate.Id = withId.ToString();
            existingCertificate.Number = @event.Number;
            existingCertificate.Status = "Draft";

            await SaveIntoReadModelAsync(withDocStore, withId, existingCertificate, cancellationToken);
        }

        private async Task ProcessEvent(DocumentsStore withDocStore, Guid withId,
            CertificateHasBeenSigned @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing CertificateHasBeenSigned event");

            var existingCertificate = await GetCertificateFromCache(withId, withDocStore, cancellationToken);

            existingCertificate.Status = "Signed";
            existingCertificate.SignedAt = @event.SignedAt.ToString();
            existingCertificate.SignedBy = @event.SignedBy;

            await SaveIntoReadModelAsync(withDocStore, withId, existingCertificate, cancellationToken);
        }

        private Task LogNotExistingHandlerForEvent(string forEventType, string forEventId)
        {
            _logger.LogInformation($"Event handler for {forEventType} has not been implemented yet; event id is: {forEventId}");

            return Task.CompletedTask;
        }

        private async Task<Application.Queries.Documents.Certificate> GetCertificateFromCache(
            Guid withId, DocumentsStore fromDocStore, CancellationToken cancellationToken)
        {
            if (_localCertificatesCache.ContainsKey(withId))
                return _localCertificatesCache[withId];

            var maybeDoc = await fromDocStore.GetByIdAsync<Application.Queries.Documents.Certificate>(withId.ToString(), cancellationToken);
            if(maybeDoc.HasValue)
            {
                _localCertificatesCache[withId] = maybeDoc.Value;
                return maybeDoc.Value;
            }

            var @new = new Application.Queries.Documents.Certificate();
            _localCertificatesCache[withId] = @new;
            return @new;
        }

        private void SaveCertificateInCache(Application.Queries.Documents.Certificate certificate, Guid withId)
        {
            _localCertificatesCache[withId] = certificate;
        }

        private async Task SaveIntoReadModelAsync(DocumentsStore withDocStore, Guid withId,
            Application.Queries.Documents.Certificate certificateToSave,
            CancellationToken cancellationToken)
        {
            await withDocStore.SaveAsync(certificateToSave, cancellationToken);
            SaveCertificateInCache(certificateToSave, withId);
        }
    }
}
