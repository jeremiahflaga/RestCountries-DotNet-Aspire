var builder = DistributedApplication.CreateBuilder(args);

/* SampleApp */
var sampleAppCache = builder.AddRedis("sampleapp-cache");

var sampleAppApiService = builder.AddProject<Projects.SampleApp_ApiService>("sampleapp-apiservice");

builder.AddProject<Projects.SampleApp_WebApp>("sampleapp-webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(sampleAppCache)
    .WaitFor(sampleAppCache)
    .WithReference(sampleAppApiService)
    .WaitFor(sampleAppApiService);

builder.Build().Run();
