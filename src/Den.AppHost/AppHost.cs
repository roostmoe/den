using Aspire.Hosting.Yarp.Transforms;

using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false);

var postgresDb = postgres.AddDatabase("postgresdb");

var migrations = builder.AddProject<Projects.Den_MigrationService>("migrations")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

var auth = builder.AddProject<Projects.Den_Auth_Api>("auth")
    .WithReference(postgresDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

var reminders = builder.AddProject<Projects.Den_Reminders_Api>("reminders")
    .WithReference(postgresDb)
    .WithReference(migrations)
    .WaitForCompletion(migrations);

var gateway = builder.AddYarp("gateway")
    .WithHostPort(builder.Configuration.GetValue<int>("Port"))
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/auth/{**catch-all}", auth)
            .WithTransformPathRemovePrefix("/auth");

        yarp.AddRoute("/reminders/{**catch-all}", reminders)
            .WithTransformPathRemovePrefix("/auth");
    });

builder.Build().Run();