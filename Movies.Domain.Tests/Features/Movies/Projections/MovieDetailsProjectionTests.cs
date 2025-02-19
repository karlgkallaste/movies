using FizzWare.NBuilder;
using FluentAssertions;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Events;
using Movies.Domain.Features.Movies.Projections;

namespace Movies.Domain.Tests.Features.Movies.Projections
{
    [TestFixture]
    public class MovieDetailsProjectionTests
    {
        private MovieDetailsProjection _projection;

        [SetUp]
        public void Setup()
        {
            _projection = new MovieDetailsProjection();
        }

        [Test]
        public void Apply_movieCreated_sets_movie_details()
        {
            var details = Builder<MovieDetails>.CreateNew().Build();
            var movieId = Guid.NewGuid();
            var @event = new MovieCreated(movieId, "Test Title", "Test Overview", DateTimeOffset.Now, MovieStatus.Released, MovieGenre.Action);

            // Act
            _projection.Apply(@event, details);

            // Assert
            details.Id.Should().Be(movieId);
            details.Title.Should().BeEquivalentTo(@event.Title);
            details.Overview.Should().BeEquivalentTo(@event.Overview);
            details.AverageRating.Should().Be(1);
            details.RatingCount.Should().Be(1);
        }
        
        [Test]
        public void Rating_count_and_average_are_set_correctly()
        {
            var details = Builder<MovieDetails>.CreateNew()
                .With(x => x.AverageRating, 4)
                .With(x => x.RatingCount, 2)
                .Build();

            // Act
            _projection.Apply(new MovieRated(details.Id, 2), details);
            _projection.Apply(new MovieRated(details.Id, 3), details);
            _projection.Apply(new MovieRated(details.Id, 4), details);

            // Assert
            details.RatingCount.Should().Be(5);
            details.AverageRating.Should().Be(3.4);
        }
    }
}