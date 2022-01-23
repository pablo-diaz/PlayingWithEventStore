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
            {
                if (eventType == NewCertificateHasBeenRegistered.ApplicationEventTypeName)
                {
                    var eventData = new NewCertificateHasBeenRegistered()
                        .Deserialize<NewCertificateHasBeenRegistered>(serializedData);

                    result = Certificate.Create(withNumber: eventData.Number).Value;
                    result.Value.SetEntityId(withId);
                }
            }

            return result;
        }
    }
}
