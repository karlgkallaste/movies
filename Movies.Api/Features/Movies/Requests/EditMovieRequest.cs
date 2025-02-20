namespace Movies.Api.Features.Movies.Requests;

public class EditMovieRequest
{
    public Guid Id { get; set; }
    public string Overview { get; set; } = null!;
    public decimal Budget { get;  set; }
    public string HomePage { get;  set; } = null!;
}