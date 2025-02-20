using Microsoft.Extensions.Configuration;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pgUser = builder.AddParameter("PgUser", secret: true);
var pgPassword = builder.AddParameter("PgPass", secret: true);

var postgres = builder.AddPostgres(name: "default", port: 5432, userName: pgUser, password: pgPassword)
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Movies_Api>("Api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithEnvironment("ConnectionStrings__Default", builder.Configuration.GetConnectionString("Default"));
builder.Build().Run();