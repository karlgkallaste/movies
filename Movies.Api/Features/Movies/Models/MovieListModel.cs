namespace Movies.Api.Features.Movies.Models;

public class MovieListModel
{
    public IEnumerable<MovieListItemModel> Items { get; set; } = new List<MovieListItemModel>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}


public class MovieListItemModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
}