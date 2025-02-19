using FizzWare.NBuilder;
using FluentAssertions;
using Marten;
using Marten.Events;
using Moq;
using Movies.Data;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Tests.Features.Movies.Commands;

[TestFixture]
public class RateMovieCommandTests
{
    private Mock<IRepository<Movie>> _movieRepositoryMock;
    private Mock<IDocumentSession> _documentSessionMock;
    private Mock<IEventStore> _eventStoreMock;
    

    [SetUp]
    public void Setup()
    {
        _movieRepositoryMock = new Mock<IRepository<Movie>>();
        _documentSessionMock = new Mock<IDocumentSession>();
        _eventStoreMock = new Mock<IEventStore>();

        _documentSessionMock.Setup(s => s.Events).Returns(_eventStoreMock.Object);
    }

    [Test]
    public async Task Handle_returns_failed_result_if_movie_is_not_found()
    {
        var command = new RateMovieCommand(Guid.NewGuid(), 5);
        _movieRepositoryMock.Setup(repo => repo.GetById(command.Id)).ReturnsAsync((Movie?)null);

        // Act
        var result =
            await RateMovieCommandHandler.Handle(command, _documentSessionMock.Object, _movieRepositoryMock.Object);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.First().Key.Should().BeEquivalentTo(nameof(Movie));
        result.Errors.First().Value.Should().BeEquivalentTo("Movie not found.");
    }

    [Test]
    public async Task Handle_returns_failed_result_if_movie_is_not_in_released_status()
    {
        var command = new RateMovieCommand(Guid.NewGuid(), 5);

        var movie = Builder<Movie>.CreateNew().With(x => x.Status, MovieStatus.InProduction).Build();
        _movieRepositoryMock.Setup(repo => repo.GetById(command.Id)).ReturnsAsync(movie);

        // Act
        var result =
            await RateMovieCommandHandler.Handle(command, _documentSessionMock.Object, _movieRepositoryMock.Object);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.First().Key.Should().BeEquivalentTo(nameof(Movie));
        result.Errors.First().Value.Should().BeEquivalentTo("Only released movies can be rated.");
    }

    [Test]
    public async Task Handle_ShouldAppendEventAndSave_WhenMovieIsReleased()
    {
        var command = new RateMovieCommand(Guid.NewGuid(), 5);
        var movie = Builder<Movie>.CreateNew().With(x => x.Status, MovieStatus.Released).Build();

        _movieRepositoryMock.Setup(repo => repo.GetById(command.Id)).ReturnsAsync(movie);

        // Act
        var result = await RateMovieCommandHandler.Handle(command, _documentSessionMock.Object, _movieRepositoryMock.Object);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _eventStoreMock.Verify(e => e.Append(command.Id, It.IsAny<MovieRated>()), Times.Once);
        _documentSessionMock.Verify(s => s.SaveChangesAsync(default), Times.Once);
    }
}