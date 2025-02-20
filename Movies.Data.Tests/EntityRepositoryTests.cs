using FizzWare.NBuilder;
using FluentAssertions;
using Marten;
using Moq;
using Movies.Domain.Features.Movies;

namespace Movies.Data.Tests;

[TestFixture]
public class EntityRepositoryTests
{
    private Mock<IDocumentSession> _mockDocumentSession;
    private EntityRepository<Movie> _repository;

    [SetUp]
    public void SetUp()
    {
        _mockDocumentSession = new Mock<IDocumentSession>();
        _repository = new EntityRepository<Movie>(_mockDocumentSession.Object);
    }

    [Test]
    public async Task GetById_returns_movie_by_id()
    {
        var expectedMovie = Builder<Movie>.CreateNew().Build();
        _mockDocumentSession.Setup(x => x.LoadAsync<Movie>(expectedMovie.Id, default)).ReturnsAsync(expectedMovie);

        // Act
        var result = await _repository.GetById(expectedMovie.Id);

        // Assert
        result.Id.Should().Be(expectedMovie.Id);
    }
    
    [Test]
    public async Task GetById_returns_null_when_movie_is_not_found()
    {
        var movieId = Guid.NewGuid();
        _mockDocumentSession.Setup(x => x.LoadAsync<Movie>(movieId, default)).ReturnsAsync((Movie?)null);

        // Act
        var result = await _repository.GetById(movieId);

        // Assert
        result.Should().BeNull();
    }
}