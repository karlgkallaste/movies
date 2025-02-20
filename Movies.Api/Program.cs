using Marten;
using Marten.Events.Projections;
using Microsoft.OpenApi.Models;
using Movies.Api.TestData;
using Movies.Data;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Projections;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movie API",
        Version = "v1",
        Description = "API for managing movies"
    });
});
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Default")!);
    options.UseNewtonsoftForSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All);
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.Projections.Snapshot<Movie>(SnapshotLifecycle.Inline);
    options.Projections.Add<MovieDetailsProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<MovieListProjection>(ProjectionLifecycle.Inline);
}).UseLightweightSessions();

builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
builder.Services.AddScoped(typeof(IProjectionRepository<>), typeof(ProjectionRepository<>));

builder.Host.UseWolverine(o =>
{
    o.UseFluentValidation();
    o.Discovery.IncludeAssembly(typeof(CreateMovieCommand).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await TestDataSeeder.Seed(app.Services);
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie API V1");
        c.RoutePrefix = "swagger"; // Serve Swagger at the root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();