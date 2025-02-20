using Marten;
using Microsoft.Extensions.Logging;
using Movies.Data;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record EditMovieCommand(Guid Id, string Overview, decimal Budget, string Homepage);

public class EditMovieCommandHandler
{
    public static async Task<Result> Handle(EditMovieCommand command, IDocumentSession session,
        IEntityRepository<Movie> movieEntityRepository, ILogger<EditMovieCommand> logger)
    {
        var movie = await movieEntityRepository.GetById(command.Id); // Need the latest information.

        if (movie == null)
        {
            logger.LogWarning("Tried to edit not found movie. ID: {Id}", command.Id);
            return Result.Failure(nameof(Movie), "Movie not found.");
        }

        session.Events.Append(command.Id, new MovieEdited(command.Id, command.Overview, command.Budget, command.Homepage));
        await session.SaveChangesAsync();

        return Result.Success();
    }
}

