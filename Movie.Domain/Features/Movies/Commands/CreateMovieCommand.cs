using Marten;
using Movie.Domain.Features.Movies.Events;

namespace Movie.Domain.Features.Movies.Commands;

public record CreateMovieCommand(Guid Id);

public class CreateMovieCommandHandler
{
    public static async Task Handle(CreateMovieCommand command, IDocumentSession session)
    {
        var @event = new MovieCreated(command.Id);
        session.Events.StartStream<Movie>(command.Id, @event);
        await session.SaveChangesAsync();
    }
}