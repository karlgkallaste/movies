using Movie.Domain.Features.Movies.Events;

namespace Movie.Domain.Features.Movies;

public class Movie
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Overview { get; private set; }
    public int Popularity { get; private set; }
    public DateTimeOffset ReleaseDate { get; private set; }
    public MovieStatus Status { get; private set; }
    public double Budget { get; private set; }
    public string HomePage { get; private set; }
    public int Rating { get; private set; }
    public Genre Genre { get; private set; }

    public void Apply(MovieCreated @event)
    {
        Id = @event.Id;
    }
}

public enum MovieStatus
{
    Released = 0,
    InProduction = 1
}

public enum Genre
{
    Action = 0,
    Drama = 1
}