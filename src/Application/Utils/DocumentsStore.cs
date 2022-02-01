using System.Threading;
using System.Threading.Tasks;

using Application.Queries.Documents;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    public abstract class DocumentsStore
    {
        private readonly string _applicationNamePrefix;

        protected DocumentsStore(string applicationNamePrefix)
        {
            this._applicationNamePrefix = applicationNamePrefix;
        }

        protected abstract Task SaveAsync(object documentToStore,
            string documentsName, CancellationToken cancellationToken);

        public Task SaveAsync<T>(T documentToStore,
                CancellationToken cancellationToken) where T: Document =>
            SaveAsync(documentToStore, GetDocumentName<T>(_applicationNamePrefix), cancellationToken);

        protected abstract Task<Maybe<T>> GetByIdAsync<T>(string documentIdToSearch,
            string documentsName, CancellationToken cancellationToken) where T : Document;

        public Task<Maybe<T>> GetByIdAsync<T>(string documentIdToSearch,
                CancellationToken cancellationToken) where T : Document =>
            GetByIdAsync<T>(documentIdToSearch, GetDocumentName<T>(_applicationNamePrefix), cancellationToken);

        private static string GetDocumentName<T>(string withAppPrefix) where T: Document
        {
            var type = typeof(T);
            if (type == typeof(Certificate)) return $"{withAppPrefix}-certificates";
            if (type == typeof(PositionOfLastEventRead)) return $"{withAppPrefix}-last-read-position";

            throw new System.NotImplementedException($"Document type '{type}' has not been setup");
        }
    }
}
