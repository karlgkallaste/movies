using Marten;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Api.TestData;

public static class TestMovieRatingSeed
{
    public static async Task Seed(IDocumentSession documentSession)
    {
        var ratings = new[]
        {
            // The Godfather
            new MovieRated(TestMovieSeed.Movie1Id, 5),
            new MovieRated(TestMovieSeed.Movie1Id, 5),
            new MovieRated(TestMovieSeed.Movie1Id, 5),
            new MovieRated(TestMovieSeed.Movie1Id, 5),

            // Inception
            new MovieRated(TestMovieSeed.Movie4Id, 4),
            new MovieRated(TestMovieSeed.Movie4Id, 5),
            new MovieRated(TestMovieSeed.Movie4Id, 5),
            new MovieRated(TestMovieSeed.Movie4Id, 4),

            // The Lion King
            new MovieRated(TestMovieSeed.Movie6Id, 4),
            new MovieRated(TestMovieSeed.Movie6Id, 2),
            new MovieRated(TestMovieSeed.Movie6Id, 5),
            new MovieRated(TestMovieSeed.Movie6Id, 1),
        };

        foreach (var rating in ratings)
        {
            documentSession.Events.Append(rating.Id, rating);
        }

        await documentSession.SaveChangesAsync();
    }
}