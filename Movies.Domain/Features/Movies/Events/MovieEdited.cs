namespace Movies.Domain.Features.Movies.Events;

public record MovieEdited(Guid Id, string Overview, decimal Budget, string HomePage);
