using Marten;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record CreateMovieCommand(string Title, string Overview, DateTimeOffset? ReleaseDate, MovieStatus Status, MovieGenre MovieGenre);

public class CreateMovieCommandHandler
{
    public static async Task<Result> Handle(CreateMovieCommand command, IDocumentSession session)
    {
        var movieId = Guid.NewGuid();
        var @event = new MovieCreated(movieId, command.Title, command.Overview, command?.ReleaseDate, command.Status, command.MovieGenre);
        session.Events.StartStream<Movie>(movieId, @event);
        await session.SaveChangesAsync();

        return Result.Success();
    }
}