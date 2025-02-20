using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Projections;

namespace Movies.Api.Features.Movies.Models;

public class MovieDetailsModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public string Homepage { get; set; }
    public MovieStatus Status { get; set; }
    public decimal Budget { get; set; }
    public double AverageRating { get; set; }
    public string Genre { get; set; }

    public MovieDetailsModel(MovieDetails details)
    {
        Id = details.Id;
        Title = details.Title;
        Overview = details.Overview;
        Homepage = details.Homepage;
        Status = details.Status;
        Budget = details.Budget;
        Genre = GenreTranslator(details.Genre);
        AverageRating = details.AverageRating;
    }

    private string GenreTranslator(MovieGenre detailsGenre)
    {
        switch (detailsGenre)
        {
            case MovieGenre.Action:
                return "Action";
            case MovieGenre.Animation:
                return "Animation";
            case MovieGenre.Crime:
                return "Crime";
            case MovieGenre.Drama:
                return "Drama";
            case MovieGenre.SciFi:
                return "SciFi";
            default:
                return "-";
        }
    }
}