using FizzWare.NBuilder;
using FluentAssertions;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Movies.Data;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Events;

namespace Movies.Domain.Tests.Features.Movies.Commands;

[TestFixture]
public class EditMovieCommandTests
{
    private Mock<IEntityRepository<Movie>> _movieRepositoryMock;
    private Mock<IDocumentSession> _documentSessionMock;
    private Mock<IEventStore> _eventStoreMock;
    private Mock<ILogger<EditMovieCommand>> _loggerMock;


    [SetUp]
    public void Setup()
    {
        _movieRepositoryMock = new Mock<IEntityRepository<Movie>>();
        _documentSessionMock = new Mock<IDocumentSession>();
        _eventStoreMock = new Mock<IEventStore>();
        _loggerMock = new Mock<ILogger<EditMovieCommand>>();
        _documentSessionMock.Setup(s => s.Events).Returns(_eventStoreMock.Object);
    }

    [Test]
    public async Task Handle_returns_failed_result_if_movie_is_not_found()
    {
        var command = new EditMovieCommand(Guid.NewGuid(), "", 2m, "");
        _movieRepositoryMock.Setup(repo => repo.GetById(command.Id)).ReturnsAsync((Movie?)null);

        // Act
        var result =
            await EditMovieCommandHandler.Handle(command, _documentSessionMock.Object, _movieRepositoryMock.Object,
                _loggerMock.Object);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.First().Key.Should().BeEquivalentTo(nameof(Movie));
        result.Errors.First().Value.Should().BeEquivalentTo("Movie not found.");
    }
    
    [Test]
    public async Task Handle_appends_movieEdited_event()
    {
        var command = new EditMovieCommand(Guid.NewGuid(), "", 2m, "");
        var movie = Builder<Movie>.CreateNew().With(x => x.Status, MovieStatus.Released).Build();

        _movieRepositoryMock.Setup(repo => repo.GetById(command.Id)).ReturnsAsync(movie);

        // Act
        var result = await EditMovieCommandHandler.Handle(command, _documentSessionMock.Object, _movieRepositoryMock.Object, _loggerMock.Object);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _eventStoreMock.Verify(e => e.Append(command.Id, It.IsAny<MovieEdited>()), Times.Once);
        _documentSessionMock.Verify(s => s.SaveChangesAsync(default), Times.Once);
    }
}