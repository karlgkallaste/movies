using Marten;
using Movies.Data;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Features.Movies.Commands;

public record RateMovieCommand(Guid Id, int Rating);

public static class RateMovieCommandHandler
{
    public static async Task<Result> Handle(RateMovieCommand command, IDocumentSession session,
        IRepository<Movie> movieRepository)
    {
        var movie = await movieRepository.GetById(command.Id);

        if (movie == null)
        {
            return Result.Failure(nameof(Movie), "Movie not found.");
        }

        if (movie.Status != MovieStatus.Released)
        {
            return Result.Failure(nameof(Movie), "Only released movies can be rated.");
        }

        session.Events.Append(command.Id, new MovieRated(command.Id, command.Rating));
        await session.SaveChangesAsync();

        return Result.Success();
    }
}