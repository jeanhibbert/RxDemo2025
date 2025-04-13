var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.RxDemoApp_ApiService>("apiservice");

//builder.AddProject<Projects.RxDemoApp_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(apiService)
//    .WithReference(cache);

builder.Build().Run();
