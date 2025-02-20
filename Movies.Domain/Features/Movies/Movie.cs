using Movies.Data;
using Movies.Domain.Features.Movies.Events;
using Newtonsoft.Json;

namespace Movies.Domain.Features.Movies;

public class Movie : IWithId
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Overview { get; private set; }
    public int Popularity { get; private set; }
    public DateTimeOffset? ReleaseDate { get; private set; }
    public MovieStatus Status { get; private set; }
    public decimal Budget { get; private set; }
    public string HomePage { get; private set; }
    public int RatedCount { get; private set; }
    public MovieGenre MovieGenre { get; private set; }

    protected Movie(){}
    private Movie(Guid id, MovieStatus status)
    {
        Id = id;
        Status = status;
    }

    public void Apply(MovieCreated @event)
    {
        Id = @event.Id;
        Title = @event.Title;
        Overview = @event.Overview;
        ReleaseDate = @event.ReleaseDate ?? null;
        Status = @event.Status;
        MovieGenre = @event.MovieGenre;
    }

    public void Apply(MovieRated @event)
    {
        RatedCount += 1;
    }

    public void Apply(MovieEdited @event)
    {
        if (!Overview.Equals(@event.Overview, StringComparison.OrdinalIgnoreCase))
        {
            Overview = @event.Overview;
        }

        Budget = @event.Budget;
        HomePage = @event.HomePage;
    }

}

public enum MovieStatus
{
    Released = 0,
    InProduction = 1
}

public enum MovieGenre
{
    Action = 0,
    Drama = 1,
    Crime = 2,
    SciFi = 3,
    Animation = 4,
}