using Aspire.Hosting.Yarp.Transforms;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithHostPort(5432)
    .WithDataVolume(isReadOnly: false);

var postgresDb = postgres.AddDatabase("postgresdb");

var migrations = builder.AddProject<Projects.Den_MigrationService>("migrations")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

var api = builder.AddProject<Projects.Den_Api>("api")
    .WithReference(postgresDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

var webClient = builder.AddViteApp("client-web", "../Den.Client.Web")
    .WithBun()
    .WithIconName("globe")
    .WithExternalHttpEndpoints();

var gateway = builder.AddYarp("gateway")
    .WithHostPort(builder.Configuration.GetValue<int>("Port"))
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute(webClient);
        yarp.AddRoute("/api/{**catch-all}", api)
            .WithTransformPathRemovePrefix("/api");
    });

builder.Build().Run();