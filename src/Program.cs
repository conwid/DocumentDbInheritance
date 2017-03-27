namespace DocumentDB.GetStarted
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Configuration;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Program
    {

        private static readonly string EndpointUri = ""; // endpoint from portal here
        private static readonly string PrimaryKey = ""; // key from portal here
        private DocumentClient client;


        static void Main(string[] args)
        {
            Program p = new Program();
            p.GetStartedDemo().Wait();
        }

        private async Task GetStartedDemo()
        {
            string databaseName = "vehicleTest";
            string collectionName = "Vehicles";
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            #region Create database
            try
            {
                await this.client.CreateDatabaseAsync(new Database { Id = databaseName });
            }
            catch { }
            #endregion
            #region Create collection
            try
            {
                DocumentCollection collectionInfo = new DocumentCollection();
                collectionInfo.Id = collectionName;
                collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
                await this.client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(databaseName), new DocumentCollection { Id = collectionName }, new RequestOptions { OfferThroughput = 400 });
            }
            catch { }
            #endregion


            Vehicle v1 = new Car { CarProperty = Guid.NewGuid().ToString() };
            Vehicle v2 = new Truck { TruckProperty = Guid.NewGuid().ToString() };

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), v1);
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), v2);

            IQueryable<Vehicle> vehicleQuery = this.client.CreateDocumentQuery<Vehicle>(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName)).Where(s => s.Id == "10");
            var results = vehicleQuery.ToList();
        }
        

        public class Vehicle
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }

            public string LicensePlate { get; set; }
        }

        public class Car : Vehicle
        {
            public string CarProperty { get; set; }
        }

        public class Truck : Vehicle
        {
            public string TruckProperty { get; set; }
        }
    }
}
