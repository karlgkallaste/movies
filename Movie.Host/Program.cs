using Projects;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Movie_Api>("Api");

builder.Build().Run();