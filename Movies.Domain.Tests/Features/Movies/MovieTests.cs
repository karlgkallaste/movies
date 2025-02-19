using FizzWare.NBuilder;
using FluentAssertions;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Tests.Features.Movies
{
    [TestFixture]
    public class MovieTests
    {
        [Test]
        public void Apply_MovieCreated_ShouldSetProperties()
        {
            var movie = Builder<Movie>.CreateNew().Build();
            var @event = new MovieCreated(movie.Id, "Test Title", "Test Overview", DateTimeOffset.Now, MovieStatus.Released, MovieGenre.Action);

            // Act
            movie.Apply(@event);

            // Assert
            movie.Title.Should().BeEquivalentTo(@event.Title);
            movie.Overview.Should().BeEquivalentTo(@event.Overview);
            movie.ReleaseDate.Should().Be(@event.ReleaseDate);
            movie.Status.Should().Be(@event.Status);
            movie.MovieGenre.Should().Be(@event.MovieGenre);
        }

        [Test]
        public void Apply_MovieRated_ShouldIncreaseRatedCount()
        {
            var movie = Builder<Movie>.CreateNew().With(x => x.RatedCount, 5).Build();
            var @event = new MovieRated(movie.Id, 5);

            // Act
            movie.Apply(@event);

            // Assert
            movie.RatedCount.Should().Be(6);
        }
    }
}