using System;
using System.Threading;
using System.Threading.Tasks;

using Domain;

using Application.Utils;
using Application.Commands.Services;

using MediatR;

using CSharpFunctionalExtensions;

namespace Application.Commands
{
    public sealed class SignCertificateCommand: IRequest<Result>
    {
        private readonly Guid _certificateId;
        private readonly string _signedBy;

        public SignCertificateCommand(Guid certificateId, string signedBy)
        {
            this._certificateId = certificateId;
            this._signedBy = signedBy;
        }

        internal sealed class SignCertificateCommandHandler :
            IRequestHandler<SignCertificateCommand, Result>
        {
            private readonly EventsStore _store;

            public SignCertificateCommandHandler(EventsStore store)
            {
                this._store = store;
            }

            public async Task<Result> Handle(SignCertificateCommand request, CancellationToken cancellationToken)
            {
                var maybeCertificate = await _store.Load(request._certificateId,
                    events => CertificateEventsApplier.Apply(withId: request._certificateId, events));
                if (maybeCertificate.HasNoValue)
                    return Result.Failure("Certificate has not been found");

                return SignCertificate(maybeCertificate.Value, request._signedBy);
            }

            private Result SignCertificate(Certificate certificate, string signedBy)
            {
                var signatureAuditResult = Audit.Create(at: DateTimeOffset.UtcNow, by: signedBy);
                if(signatureAuditResult.IsFailure)
                    return Result.Failure(signatureAuditResult.Error);

                var signatureResult = certificate.Sign(signatureAuditResult.Value);
                if(signatureResult.IsFailure)
                    return Result.Failure(signatureResult.Error);

                return Result.Success();
            }
        }
    }
}
