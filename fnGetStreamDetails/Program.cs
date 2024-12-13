using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(
{
	string connectionString = Environment.GetEnvironmentVariable("CosmoDBConnection");
	return new CosmoClient(connectionString);
});

builder.Build().Run();