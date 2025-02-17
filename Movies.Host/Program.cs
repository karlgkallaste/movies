using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres("default")
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Movies_Api>("Api")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);
builder.Build().Run();