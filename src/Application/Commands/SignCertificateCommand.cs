using System;
using System.Threading;
using System.Threading.Tasks;

using Domain;

using Application.Utils;
using Application.Commands.Services;
using Application.Commands.StoreEvents;

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

                var signatureResult = SignCertificate(maybeCertificate.Value, request._signedBy);
                if (signatureResult.IsFailure)
                    return signatureResult;

                await _store.AppendEvent(aggregateToAppend: maybeCertificate.Value,
                    @event: CreateEventToStore(maybeCertificate.Value));

                return Result.Success();
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

            private CertificateHasBeenSigned CreateEventToStore(Certificate forSignedCertificate) =>
                new CertificateHasBeenSigned {
                    SignedAt = forSignedCertificate.SignedAudit.Value.At,
                    SignedBy = forSignedCertificate.SignedAudit.Value.By
                };
        }
    }
}
