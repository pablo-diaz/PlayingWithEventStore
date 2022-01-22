using System.Threading.Tasks;
using System.Collections.Generic;

using Domain.Common;

using MediatR;

namespace Application.Utils
{
    public sealed class EventDispatcher
    {
        private readonly IMediator _mediator;

        public EventDispatcher(IMediator mediator)
        {
            this._mediator = mediator;
        }

        internal async Task DispatchDomainEvents(IEnumerable<IDomainEvent> events)
        {
            foreach (var theEvent in events)
                await this._mediator.Publish(theEvent);
        }
    }
}
