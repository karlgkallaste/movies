using Marten;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record CreateMovieCommand(string Title, string Overview, MovieStatus Status);

public class CreateMovieCommandHandler
{
    public static async Task<Result> Handle(CreateMovieCommand command, IDocumentSession session)
    {
        var movieId = Guid.NewGuid();
        var @event = new MovieCreated(movieId, command.Title, command.Overview, command.Status);
        session.Events.StartStream<Movie>(movieId, @event);
        await session.SaveChangesAsync();

        return Result.Success();
    }
}