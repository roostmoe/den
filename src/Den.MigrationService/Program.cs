using Den.Infrastructure.Persistence;
using Den.MigrationService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults(isApi: false);
builder.Services.AddHostedService<Worker>();
builder.AddNpgsqlDbContext<DenDbContext>(connectionName: "postgresdb");

var host = builder.Build();
host.Run();