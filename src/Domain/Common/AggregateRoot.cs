using System.Collections.Generic;

using CSharpFunctionalExtensions;

namespace Domain.Common
{
    public abstract class AggregateRoot : Entity<string>
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        protected void RaiseDomainEvent(IDomainEvent domainEventToRaise)
        {
            this._domainEvents.Add(domainEventToRaise);
        }

        public void ClearDomainEvents()
        {
            this._domainEvents.Clear();
        }
    }
}
