using Marten;
using Movies.Domain.Features.Movies;
using Wolverine;

namespace Movies.Api.TestData;

public static class TestDataSeeder
{
    public static async Task Seed(IServiceProvider appServices)
    {
        using var scope = appServices.CreateScope();
        var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        if (documentSession.Query<Movie>().Any())
        {
            return;
        }

        await TestMovieSeed.Seed(documentSession);
    }
}