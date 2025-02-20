using Marten;
using Marten.Events.Aggregation;
using Movies.Data;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Projections;

public class MovieDetails : IProjection
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public string Homepage { get; set; }
    public MovieStatus Status { get; set; }
    public decimal Budget { get; set; }
    public double AverageRating { get; set; }
    public MovieGenre Genre { get; set; }
    public int RatingCount { get; set; }
}

public class MovieDetailsProjection : SingleStreamProjection<MovieDetails>
{
    public void Apply(MovieCreated @event, MovieDetails details)
    {
        details.Id = @event.Id;
        details.Title = @event.Title;
        details.Overview = @event.Overview;
        details.ReleaseDate = @event.ReleaseDate;
        details.Status = @event.Status;
        details.Genre = @event.MovieGenre;
    }

    public void Apply(MovieRated @event, MovieDetails details)
    {
        details.RatingCount++;
        details.AverageRating =
            Math.Round((details.AverageRating * (details.RatingCount - 1) + @event.Rating) / details.RatingCount, 2);
    }
    
    public void Apply(MovieEdited @event, MovieDetails details)
    {
        details.Overview = @event.Overview;
        details.Homepage = @event.HomePage;
        details.Budget = @event.Budget;
    }
}