using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace fnPostDataStorage
{
    public class fnPostDataStorage
    {
        private readonly ILogger<Function1> _logger;

        public fnPostDataStorage(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("dataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processando um stream do blob.");
			
			if(!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
			{
				return new BadRequestObjectResult("froneça o file-type!");
			}
				
			var fileType = fileTypeHeader.ToString();
			var form = await req.ReadFormAsync();
			var file = form.Files["file"];
				
			if(file == null || file.Length == 0)
			{
				return new BadRequestObjectResult("arquivo não foi enviado!");
			}
				
			string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
			string containerName = fileType;
			BlobClient blobClient = new BlobClient(connectionString, containerName, file.FileName);
			BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
			
			await containerClient.CreateIfNotExistsAsync();
			await containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);
			
			string blobName = file.FileName;
			var blob = containerClient.GetBlobClient(blobName);
			
			using (var stream = file.OpenReadStream())
			{
				await blob.UploadAsync(stream, true);
			}
			
			_logger.LogInformation($"Arquivo {file.FileName} armazenado com sucesso");
			
            return new OkObjectResult(
			{
				Message = "Arquivo armazenado com sucesso",
				BlobUri = blob.Uri
		    });
        }
    }
}
