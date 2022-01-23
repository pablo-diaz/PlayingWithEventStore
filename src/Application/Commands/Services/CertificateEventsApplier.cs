using System;
using System.Collections.Generic;

using Domain;

using Application.Utils;
using Application.Commands.StoreEvents;

using CSharpFunctionalExtensions;

namespace Application.Commands.Services
{
    internal static class CertificateEventsApplier
    {
        public static Maybe<Certificate> Apply(Guid withId, List<(string eventType, string serializedData)> events)
        {
            var result = Maybe<Certificate>.None;
            foreach (var (eventType, serializedData) in events)
                result = Apply(result, withId, eventType, serializedData);

            return result;
        }

        private static Certificate Apply(Maybe<Certificate> maybeCertificate, Guid withId, string eventType, string serializedData)
        {
            if (eventType == NewCertificateHasBeenRegistered.ApplicationEventTypeName)
                return New(serializedData, withId);

            if (eventType == CertificateHasBeenSigned.ApplicationEventTypeName)
                return Sign(maybeCertificate.Value, serializedData);

            throw new NotImplementedException($"Unknown Event Type: {eventType}");
        }

        private static Certificate New(string fromSerializedData, Guid withId)
        {
            var eventData = EventInStore.Deserialize<NewCertificateHasBeenRegistered>(fromSerializedData);

            var certificate = Certificate.Create(withNumber: eventData.Number).Value;
            certificate.SetEntityId(withId);

            return certificate;
        }

        private static Certificate Sign(Certificate certificateToSign, string fromSerializedData)
        {
            var eventData = EventInStore.Deserialize<CertificateHasBeenSigned>(fromSerializedData);
            certificateToSign.Sign(Audit.Create(at: eventData.SignedAt, by: eventData.SignedBy).Value);

            return certificateToSign;
        }
    }
}
