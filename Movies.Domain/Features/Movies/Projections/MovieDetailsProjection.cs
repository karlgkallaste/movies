using Marten;
using Marten.Events.Aggregation;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Projections;

public class MovieDetails
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public double AverageRating { get; set; }
    public int RatingCount { get; set; }
}

public class MovieDetailsProjection : SingleStreamProjection<MovieDetails>
{
    public void Apply(MovieCreated @event, MovieDetails details)
    {
        details.Id = @event.Id;
        details.Title = @event.Title;
        details.Overview = @event.Overview;
    }

    public void Apply(MovieRated @event, MovieDetails details)
    {
        details.RatingCount++;
        details.AverageRating = Math.Round((details.AverageRating * (details.RatingCount - 1) + @event.Rating) / details.RatingCount, 2);

    }
}