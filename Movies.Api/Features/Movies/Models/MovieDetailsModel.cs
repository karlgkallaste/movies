using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Projections;

namespace Movies.Api.Features.Movies.Models;

public class MovieDetailsModel
{
    public MovieDetailsModel(MovieDetails details)
    {
        Id = details.Id;
        Title = details.Title;
        Overview = details.Overview;
        AverageRating = details.AverageRating;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public string Homepage { get; set; }
    public MovieStatus Status { get; set; }
    public decimal Budget { get; set; }
    public double AverageRating { get; set; }
    public MovieGenre Genre { get; set; }
}