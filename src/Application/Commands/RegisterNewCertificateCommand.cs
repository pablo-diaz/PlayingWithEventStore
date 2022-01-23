using System.Threading;
using System.Threading.Tasks;

using Domain;

using Application.Utils;
using Application.Commands.DTOs;
using Application.Commands.StoreEvents;

using MediatR;

using Newtonsoft.Json;

using CSharpFunctionalExtensions;

namespace Application.Commands
{
    public sealed class RegisterNewCertificateCommand: IRequest<Result<RegisterNewCertificateResultDTO>>
    {
        private readonly string _certNumber;

        public RegisterNewCertificateCommand(string certNumber)
        {
            this._certNumber = certNumber;
        }

        internal sealed class RegisterNewCertificateCommandHandler :
            IRequestHandler<RegisterNewCertificateCommand, Result<RegisterNewCertificateResultDTO>>
        {
            private readonly EventsStore _store;

            public RegisterNewCertificateCommandHandler(EventsStore store)
            {
                this._store = store;
            }

            public async Task<Result<RegisterNewCertificateResultDTO>> Handle(
                RegisterNewCertificateCommand request, CancellationToken cancellationToken)
            {
                var newCertificateResult = Certificate.Create(withNumber: request._certNumber);
                if (newCertificateResult.IsFailure)
                    return Result.Failure<RegisterNewCertificateResultDTO>(newCertificateResult.Error);

                await _store.AppendEvent(aggregateToAppend: newCertificateResult.Value,
                    @event: CreateEventToStore(newCertificateResult.Value));

                return new RegisterNewCertificateResultDTO {
                    Id = newCertificateResult.Value.Id.ToString()
                };
            }

            private NewCertificateHasBeenRegistered CreateEventToStore(Certificate forRegisteredCertificate) =>
                new NewCertificateHasBeenRegistered {
                    Number = forRegisteredCertificate.Number
                };
        }
    }
}
