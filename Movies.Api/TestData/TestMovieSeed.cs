using Marten;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Api.TestData;

public static class TestMovieSeed
{
    public static async Task Seed(IDocumentSession documentSession)
    {
        var movies = new[]
        {
            new MovieCreated(Guid.NewGuid(),"The Godfather", "One of the best movies made.", MovieStatus.Released),
            new MovieCreated(Guid.NewGuid(),"The Godfather 2", "One of the best movies made.", MovieStatus.Released),
        };

        foreach (var movie in movies)
        {
            documentSession.Events.StartStream<Movie>(movie.Id, movie);
        }

        await documentSession.SaveChangesAsync();
    }
}