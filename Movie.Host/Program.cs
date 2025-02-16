using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresDb = builder.AddPostgres("default");

builder.AddProject<Movie_Api>("Api")
    .WithReference(postgresDb);
builder.Build().Run();