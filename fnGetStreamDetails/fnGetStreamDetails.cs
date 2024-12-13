using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace fnGetStreamDetails
{
    public class fnGetStreamDetails
    {
        private readonly ILogger<fnGetStreamDetails> _logger;
		private readonly CosmosClient _cosmosClient;

        public fnGetStreamDetails(ILogger<fnGetStreamDetails> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
			_cosmosClient = cosmosClient;
        }

        [Function("stream_details")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            var container = _cosmosClient.GetContainer("dioflixdb", "streamsfromdioflixdb");
			var id = req.Query["id"];
			var query = new QueryDefinition($"SELECT * FROM c WHERE c.id = '{id}'");
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
			await responseMessage.WriteAsJsonAsync(results.FirstOrDefault());
			
			return responseMessage;
        }
    }
}