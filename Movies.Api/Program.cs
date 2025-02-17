using Marten;
using Marten.Events.Projections;
using Movies.Api.TestData;
using Movies.Data;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Projections;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("Default")!);
    options.UseNewtonsoftForSerialization();
    options.AutoCreateSchemaObjects = AutoCreate.All;
    options.Projections.Add<MovieDetailsProjection>(ProjectionLifecycle.Inline);
}).UseLightweightSessions();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();