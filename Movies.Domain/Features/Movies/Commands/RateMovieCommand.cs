using Marten;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record RateMovieCommand(Guid Id, int Rating);


public class RateMovieCommandHandler
{
    public static async Task<Result> Handle(RateMovieCommand command, IDocumentSession session)
    {
        session.Events.Append(command.Id, new MovieRated(command.Id, command.Rating));
        await session.SaveChangesAsync();

        return Result.Success();
    }
}
