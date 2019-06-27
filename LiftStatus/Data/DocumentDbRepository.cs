using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LiftStatus.Data
{
    public class DocumentDbRepository<T> where T : class
    {

        private static readonly string Endpoint = "https://localhost:8081";
        private static readonly string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static readonly string DatabaseId = "lift-notify";
        private static string CollectionId;
        private static DocumentClient client;

        public DocumentDbRepository(string collectionId)
        {
            if (string.IsNullOrEmpty(collectionId))
                throw new Exception("CollectionId is required to instantiate the Document Db Repository");

            CollectionId = collectionId;

            Initialize();

        }
            

        public async Task<T> GetItemAsync(string id, string partitionKeyValue)
        {
            try
            {
                var url = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);

                Document document = await client.ReadDocumentAsync(url,
                    new RequestOptions
                    {
                        PartitionKey = new PartitionKey(partitionKeyValue)
                    });

                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, string partitionKeyValue)
        {

            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions
                    {
                        EnableCrossPartitionQuery = true
                    })                
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(T item, string partitionKeyValue)
        {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item,
                new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKeyValue)
                });
        }

        public async Task<Document> UpdateItemAsync(string id, T item, string partitionKeyValue)
        {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item,
                new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKeyValue)
                });
        }

        public async Task DeleteItemAsync(string id, string partitionKeyValue)
        {
            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id),
                new RequestOptions
                {
                    PartitionKey = new PartitionKey(partitionKeyValue)
                });
        }

        public void Initialize()
        {
            client = new DocumentClient(new Uri(Endpoint), Key);
            
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
