using Marten.Events.Aggregation;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Projections;

public class MovieListItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}

public class MovieListProjection : SingleStreamProjection<MovieListItem>
{
    public void Apply(MovieCreated @event, MovieDetails details)
    {
        details.Id = @event.Id;
        details.Title = @event.Title;
    }
}