using System.Threading;
using System.Threading.Tasks;

using Application.Queries.Documents;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public abstract class DocumentsStore
    {
        protected abstract Task SaveAsync(object documentToStore,
            string documentsName, CancellationToken cancellationToken);

        public Task SaveAsync<T>(T documentToStore,
                CancellationToken cancellationToken) where T: Document =>
            SaveAsync(documentToStore, GetDocumentName<T>(), cancellationToken);

        protected abstract Task<Maybe<T>> GetByIdAsync<T>(string documentIdToSearch,
            string documentsName, CancellationToken cancellationToken) where T : Document;

        public Task<Maybe<T>> GetByIdAsync<T>(string documentIdToSearch,
                CancellationToken cancellationToken) where T : Document =>
            GetByIdAsync<T>(documentIdToSearch, GetDocumentName<T>(), cancellationToken);

        private static string GetDocumentName<T>() where T: Document
        {
            var type = typeof(T);
            if (type == typeof(Certificate)) return "certificates";

            throw new System.NotImplementedException($"Document type '{type}' has not been setup");
        }
    }
}
