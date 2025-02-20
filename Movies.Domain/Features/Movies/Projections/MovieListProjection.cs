using Marten.Events.Aggregation;
using Movies.Data;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Projections;

public class MovieListItem : IProjection
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public int Popularity { get; set; }
}

public class MovieListProjection : SingleStreamProjection<MovieListItem>
{
    public void Apply(MovieCreated @event, MovieListItem listItem)
    {
        listItem.Id = @event.Id;
        listItem.Title = @event.Title;
        listItem.Overview = @event.Overview;
        listItem.ReleaseDate = @event.ReleaseDate;
        listItem.Popularity = listItem.Popularity;
    }

    public void Apply(MovieEdited @event, MovieListItem listItem)
    {
        listItem.Overview = @event.Overview;
    }
}