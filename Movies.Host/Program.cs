using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres("default")
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Movies_Api>("Api")
    .WithReference(postgresDb)
    .WaitFor(postgresDb)
    .WithEnvironment("ConnectionStrings__Default", builder.Configuration.GetConnectionString("Default"));
builder.Build().Run();