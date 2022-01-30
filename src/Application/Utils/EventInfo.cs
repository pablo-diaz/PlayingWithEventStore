using System;

using Domain.Common;

using Application.Commands.StoreEvents;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public class EventInformation<T> where T: AggregateRoot
    {
        public class Information
        {
            public Guid Id { get; }
            public EventInStore Data { get; }

            internal Information(Guid id, EventInStore data)
            {
                Id = id;
                Data = data;
            }
        }

        public string OriginalId { get; }
        public string Type { get; }
        public Maybe<Information> MaybeParsedEvent { get; }

        internal EventInformation(string originalId, string eventType, Maybe<string> serializedData)
        {
            OriginalId = originalId;
            Type = eventType;
            MaybeParsedEvent = ParseEvent(originalId, eventType, serializedData);
        }

        private static Maybe<Information> ParseEvent(string originalId, string eventType, Maybe<string> serializedData)
        {
            if(serializedData.HasNoValue)
                return Maybe<Information>.None;

            try
            {
                var id = Guid.Parse(originalId.Replace(Utilities.GetStreamPrefix<T>(), ""));

                if (eventType == NewCertificateHasBeenRegistered.ApplicationEventTypeName)
                    return new Information(id, EventInStore.Deserialize<NewCertificateHasBeenRegistered>(serializedData.Value));

                if (eventType == CertificateHasBeenSigned.ApplicationEventTypeName)
                    return new Information(id, EventInStore.Deserialize<CertificateHasBeenSigned>(serializedData.Value));

                return Maybe<Information>.None;
            }
            catch
            {
                return Maybe<Information>.None;
            }
        }
    }
}
