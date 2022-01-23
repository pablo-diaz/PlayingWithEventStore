using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Domain;
using Domain.Common;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public abstract class EventsStore
    {
        internal Task AppendEvent<T>(T aggregateToAppend, string eventType, string serializedEventData,
                CancellationToken cancellationToken = new CancellationToken()) where T : AggregateRoot =>
            SaveAsync(streamName: BuildStreamName(aggregateToAppend), eventType,
                currentVersion: 1, serializedEventData, cancellationToken);

        internal Task<Maybe<T>> Load<T>(string streamName, Guid byId,
            CancellationToken cancellationToken = new CancellationToken()) where T: AggregateRoot
        {
            throw new NotImplementedException();
        }

        protected abstract Task SaveAsync(string streamName, string eventType,
            int currentVersion, string serializedEventData, CancellationToken cancellationToken);

        protected abstract Task<IEnumerable<string>> LoadEventsAsync(CancellationToken cancellationToken);

        private string BuildStreamName<T>(T forAggregate) where T : AggregateRoot =>
            forAggregate switch {
                Certificate certificate => $"Certificate-{forAggregate.Id}",
                _ => throw new NotImplementedException("Unknown Aggregate")
        };
    }
}
