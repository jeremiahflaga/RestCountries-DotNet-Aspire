using Microsoft.Extensions.Configuration;
using RestCountries.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var isIntegrationTest = args.Any(x => x == "--integration-test");

/* SampleApp */
var sampleAppCache = builder.AddRedis("sampleapp-cache");

var sampleAppApiService = builder.AddProject<Projects.SampleApp_ApiService>("sampleapp-apiservice");

builder.AddProject<Projects.SampleApp_WebApp>("sampleapp-webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(sampleAppCache)
    .WaitFor(sampleAppCache)
    .WithReference(sampleAppApiService)
    .WaitFor(sampleAppApiService);

var restCountriesWebApi = builder.AddProject<Projects.RestCountries_WebApi>("restcountries-apiservice");

if (isIntegrationTest)
    AddTestEnvironmentVariablesForWebApi(restCountriesWebApi);


builder.Build().Run();

static void AddTestEnvironmentVariablesForWebApi(IResourceBuilder<ProjectResource> restCountriesWebApi)
{
    // Use appsettings for integration testing, e.g. use test database
    var appsettingsFileName = Path.Combine("AppSettings", "IntegrationTest_RestCountries_AspNet_WebApi.appsettings.json");
    var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(appsettingsFileName, false, false);
    var environmentVariables = JsonToEnvironmentConverter.Convert(configBuilder);
    foreach (var envVar in environmentVariables)
    {
        var key = envVar.Key;
        var value = envVar.Value.ToString();

        if (key.Contains("ConnectionStrings") && !value.Contains("TEST_DB"))
            throw new Exception("Please use test database for integration testing.");

        restCountriesWebApi.WithEnvironment(key, value);
    }
}