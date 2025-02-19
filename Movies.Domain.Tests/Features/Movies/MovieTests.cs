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
            var @event = new MovieCreated(movie.Id, "Test Title", "Test Overview", MovieStatus.Released);

            // Act
            movie.Apply(@event);

            // Assert
            movie.Title.Should().BeEquivalentTo(@event.Title);
            movie.Overview.Should().BeEquivalentTo(@event.Overview);
            movie.Status.Should().Be(@event.Status);
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