using Movie.Domain.Features.Movies.Events;

namespace Movie.Domain.Features.Movies;

public class Movie
{
    public Guid Id { get; set; }

    public void Apply(MovieCreated @event)
    {
        Id = @event.Id;
    }
}