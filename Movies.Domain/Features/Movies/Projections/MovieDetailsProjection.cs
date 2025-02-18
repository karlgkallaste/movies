using Marten;
using Marten.Events.Aggregation;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Projections;

public class MovieDetails
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int AverageRating { get; set; }
    public int RatingCount { get; set; }
}

public class MovieDetailsProjection : SingleStreamProjection<MovieDetails>
{
    public void Apply(MovieCreated @event, MovieDetails details)
    {
        details.Id = @event.Id;
        details.Title = @event.Title;
    }

    public void Apply(MovieRated @event, MovieDetails details)
    {
        details.AverageRating = (details.AverageRating * details.RatingCount + @event.Rating) / (details.RatingCount + 1);
        details.RatingCount++;
    }
}