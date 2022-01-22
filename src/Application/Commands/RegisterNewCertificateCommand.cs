using System.Threading;
using System.Threading.Tasks;

using Domain;

using Application.Commands.DTOs;

using MediatR;

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
            public Task<Result<RegisterNewCertificateResultDTO>> Handle(
                RegisterNewCertificateCommand request, CancellationToken cancellationToken)
            {
                var newCertificateResult = Certificate.Create(withNumber: request._certNumber);
                return newCertificateResult.IsFailure
                    ? Task.FromResult(Result.Failure<RegisterNewCertificateResultDTO>(newCertificateResult.Error))
                    : Task.FromResult(Result.Success(new RegisterNewCertificateResultDTO {
                        Id = newCertificateResult.Value.Id
                    }));
            }
        }
    }
}
