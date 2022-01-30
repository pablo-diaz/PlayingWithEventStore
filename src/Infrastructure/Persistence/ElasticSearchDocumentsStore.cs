using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Utils;

using Infrastructure.ConfigDTOs;

using Microsoft.Extensions.Options;

using Nest;

using CSharpFunctionalExtensions;

namespace Infrastructure.Persistence
{
    public class ElasticSearchDocumentsStore : DocumentsStore
    {
        private readonly ElasticClient _elasticSearchClient;

        public ElasticSearchDocumentsStore(IOptions<ElasticSearchOptions> options)
        {
            _elasticSearchClient = new ElasticClient(new System.Uri(options.Value.ServiceURL));
        }

        protected override Task SaveAsync(object documentToStore,
                string documentsName, CancellationToken cancellationToken) =>
            _elasticSearchClient.IndexAsync(document: documentToStore,
                opts => opts.Index(documentsName), ct: cancellationToken);

        protected override async Task<Maybe<T>> GetByIdAsync<T>(string documentIdToSearch,
            string documentsName, CancellationToken cancellationToken)
        {
            var result = await _elasticSearchClient.SearchAsync<T>(s => s.Query(q =>
                q.Bool(b =>
                    b.Must(ms =>
                        ms.Match(ma =>
                            ma.Field(f => f.Id)
                            .Query(documentIdToSearch)
                        )
                    )
                )
            ));

            return result.Documents.Any()
                ? result.Documents.First()
                : Maybe<T>.None;
        }
    }
}
