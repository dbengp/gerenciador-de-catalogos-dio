using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace fnGetStreams
{
    public class fnGetStreams
    {
        private readonly ILogger<fnGetStreams> _logger;
		private readonly CosmosClient _cosmosClient;

        public fnGetStreamDetails(ILogger<fnGetStreams> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
			_cosmosClient = cosmosClient;
        }

        [Function("all_streams")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var container = _cosmosClient.GetContainer("dioflixdb", "streamsfromdioflixdb");
			var id = req.Query["id"];
			var query = new QueryDefinition($"SELECT * FROM c");
			var result = container.GetItemQueryIterator<StreamResult>(query);
			var results = new List<StreamResult>();
			while(result.HasMoreResults)
			{
				foreach(var item in await result.ReadNextAsync())
				{
					results.Add(item);
				}
			}
			var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
			await responseMessage.WriteAsJsonAsync(results);
			
			return responseMessage;
        }
    }
}
