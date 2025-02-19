using Marten;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Api.TestData;

public static class TestMovieSeed
{
    public static readonly Guid Movie1Id = Guid.Parse("e53fd576-8111-4b2d-84b7-a9dd0db0872d");
    public static readonly Guid Movie2Id = Guid.Parse("1509ae77-8771-449c-a584-89a46fba28c0");
    public static readonly Guid Movie3Id = Guid.Parse("4db91212-94b5-498e-91df-9b398babfe3a");
    public static readonly Guid Movie4Id = Guid.Parse("cbd9320f-d4d6-4ff9-a297-d96201455d9d");
    public static readonly Guid Movie5Id = Guid.Parse("18f78ee9-bfb0-4ae6-976d-3424d5652a25");
    public static readonly Guid Movie6Id = Guid.Parse("b25a70ff-fa56-4a38-9241-d4a27bd0dca4");

    public static async Task Seed(IDocumentSession documentSession)
    {
        var movies = new[]
        {
            new MovieCreated(Movie1Id, "The Godfather", "One of the best movies ever made. A classic mafia story.", new DateTimeOffset(1972, 3, 24, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.Drama),
            new MovieCreated(Movie2Id, "The Shawshank Redemption", "A man wrongly imprisoned for murder forms a life-changing bond with a fellow inmate.",new DateTimeOffset(1994, 9, 23, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.Drama),
            new MovieCreated(Movie3Id, "The Dark Knight", "The intertwining tales of crime and redemption in Los Angeles.", new DateTimeOffset(1994, 10, 14, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.Crime),
            new MovieCreated(Movie4Id, "Inception", "A skilled thief is given a chance to have his past crimes erased by planting an idea into a target's subconscious.", new DateTimeOffset(2010, 7, 16, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.SciFi),
            new MovieCreated(Movie5Id, "The Matrix", "A computer hacker learns the world around him is a simulation and joins a group of rebels to break free.", new DateTimeOffset(1999, 3, 31, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.SciFi),
            new MovieCreated(Movie6Id, "The Lion King", "A young lion cub's journey to become king of the Pride Lands.", new DateTimeOffset(1994, 6, 24, 0, 0, 0, TimeSpan.Zero), MovieStatus.Released, MovieGenre.Animation),
        };

        foreach (var movie in movies)
        {
            documentSession.Events.StartStream<Movie>(movie.Id, movie);
        }

        await documentSession.SaveChangesAsync();
    }
}