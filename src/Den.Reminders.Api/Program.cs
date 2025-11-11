using Den.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// builder.AddNpgsqlDbContext<WebApiContext>(connectionName: "postgresdb");

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

app.Run();