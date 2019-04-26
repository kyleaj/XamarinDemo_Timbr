using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Data;
using System.Diagnostics;
using Microsoft.Azure.Documents.Linq;
using Xamarin.Forms;

namespace Timbr
{
    public static class CosmosDBService
    {
        static DocumentClient docClient = null;
        private static Random rng = new Random();

        static readonly string databaseName = "timbr";
        static readonly string collectionName = "trees";

        static async Task<bool> Initialize()
        {
            if (docClient != null)
                return true;

            try
            {
                docClient = new DocumentClient(new Uri(APIKeys.CosmosEndpointUrl), APIKeys.CosmosAuthKey);

                // Create the database - this can also be done through the portal
                await docClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseName });

                // Create the collection - make sure to specify the RUs - has pricing implications
                // This can also be done through the portal
                await docClient.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    new DocumentCollection { Id = collectionName },
                    new RequestOptions { OfferThroughput = 400 }
                );

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                docClient = null;

                return false;
            }

            return true;
        }

        public async static Task<List<TreeProfile>> GetTrees()
        {
            var trees = new List<TreeProfile>();

            if (!await Initialize())
                return trees;

            var treeQuery = docClient.CreateDocumentQuery<TreeProfile>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                new FeedOptions { MaxItemCount = -1 })
                .AsDocumentQuery();

            while (treeQuery.HasMoreResults)
            {
                var queryResults = await treeQuery.ExecuteNextAsync<TreeProfile>();

                trees.AddRange(queryResults);
            }
            
            return trees;
        }

        public async static Task<TreeProfile> AddTree(TreeProfile tree)
        {

            if (!await Initialize())
                return null;

            await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), tree);
            return tree;
        }

    }
}
