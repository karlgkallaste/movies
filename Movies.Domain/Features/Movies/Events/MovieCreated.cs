namespace Movies.Domain.Features.Movies.Events;

public record MovieCreated(Guid Id, string Title, string Overview, DateTimeOffset? ReleaseDate, MovieStatus Status, MovieGenre MovieGenre);