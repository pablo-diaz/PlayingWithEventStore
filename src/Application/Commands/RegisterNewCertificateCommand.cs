using System.Threading;
using System.Threading.Tasks;

using Domain;

using Application.Utils;
using Application.Commands.DTOs;

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
                    eventType: "NewCertificateHasBeenRegistered",
                    serializedEventData: CreateEvent(newCertificateResult.Value));

                return new RegisterNewCertificateResultDTO {
                    Id = newCertificateResult.Value.Id.ToString()
                };
            }

            private string CreateEvent(Certificate forRegisteredCertificate) =>
                JsonConvert.SerializeObject(new {
                    ApplicationEventVersion = 1,
                    forRegisteredCertificate.Number
                });
        }
    }
}
