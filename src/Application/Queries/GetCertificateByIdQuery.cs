using System.Threading;
using System.Threading.Tasks;

using Application.Utils;
using Application.Queries.DTOs;
using Application.Queries.Documents;

using CSharpFunctionalExtensions;

using MediatR;

namespace Application.Queries
{
    public sealed class GetCertificateByIdQuery: IRequest<Result<Maybe<CertificateInfoDTO>>>
    {
        private readonly string _id;

        public GetCertificateByIdQuery(string id)
        {
            this._id = id;
        }

        internal sealed class GetCertificateByIdQueryHandler
            : IRequestHandler<GetCertificateByIdQuery, Result<Maybe<CertificateInfoDTO>>>
        {
            private readonly DocumentsStore _store;

            public GetCertificateByIdQueryHandler(DocumentsStore store)
            {
                this._store = store;
            }

            public async Task<Result<Maybe<CertificateInfoDTO>>> Handle(
                GetCertificateByIdQuery request, CancellationToken cancellationToken)
            {
                var maybeCertFound = await _store.GetByIdAsync<Certificate>(request._id, cancellationToken);
                return maybeCertFound.HasValue
                    ? Maybe<CertificateInfoDTO>.From(Map(maybeCertFound.Value))
                    : Maybe<CertificateInfoDTO>.None;
            }

            private CertificateInfoDTO Map(Certificate from) =>
                new CertificateInfoDTO {
                    Number = from.Number,
                    Status = from.Status,
                    SignedAt = from.SignedAt,
                    SignedBy = from.SignedBy
                };
        }
    }
}
