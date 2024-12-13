using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;




namespace fnPostDataBase
{
    public class fnPostDataBase
    {
        private readonly ILogger<fnPostDataBase> _logger;

        public fnPostDataBase(ILogger<fnPostDataBase> logger)
        {
            _logger = logger;
        }

        [Function("stream")]
		[CosmosDBOutput("%DatabaseName%", "%ContainerName%", Connection = "CosmoDBConnection", CreateIfNotExists = true, PartitionKey = "id")]
        public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
			StreamRequest stream = null;
			
			var content = await new StreamReader(req.Body).ReadToEndAsync();
			
			try
			{
				stream = JsonConvert.DeserializeObject<StreamRequest>(content);
			}
			catch(Exception ex)
			{
				return new BadRequestObjectResult("erro ao deserializar: " + ex.Message);
			}
			
			return JsonConvert.SerializeObject(stream);
        }
    }
}
