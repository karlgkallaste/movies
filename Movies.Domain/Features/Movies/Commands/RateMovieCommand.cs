using Marten;
using Microsoft.Extensions.Logging;
using Movies.Data;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record RateMovieCommand(Guid Id, int Rating);

public static class RateMovieCommandHandler
{
    public static async Task<Result> Handle(RateMovieCommand command, IDocumentSession session,
        IRepository<Movie> movieRepository, ILogger<RateMovieCommand> logger)
    {
        var movie = await movieRepository.GetById(command.Id); // Need the latest information.

        if (movie == null)
        {
            logger.LogWarning("Tried to rate not found movie. ID: {Id}", command.Id);
            return Result.Failure(nameof(Movie), "Movie not found.");
        }

        if (movie.Status != MovieStatus.Released)
        {
            logger.LogWarning("Tried to rate a unreleased movie. ID: {Id}", command.Id);
            return Result.Failure(nameof(Movie), "Only released movies can be rated.");
        }

        session.Events.Append(command.Id, new MovieRated(command.Id, command.Rating));
        await session.SaveChangesAsync();

        return Result.Success();
    }
}