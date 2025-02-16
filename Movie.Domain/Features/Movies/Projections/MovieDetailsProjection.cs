using Marten.Events.Aggregation;
using Movie.Domain.Features.Movies.Events;

namespace Movie.Domain.Features.Movies.Projections;

public class MovieDetails
{
    public Guid Id { get; set; }
}

public class MovieDetailsProjection : SingleStreamProjection<MovieDetails>
{
    public void Apply(MovieCreated @event, MovieDetails details)
    {
        details.Id = @event.Id;
    }
}