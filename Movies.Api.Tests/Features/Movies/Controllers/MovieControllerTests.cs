using FizzWare.NBuilder;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Marten;
using Marten.Linq;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
using Movies.Api.Features.Movies.Controllers;
using Movies.Api.Features.Movies.Models;
using Movies.Api.Features.Movies.Requests;
using Movies.Data;
using Movies.Domain.Features.Movies;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Projections;
using Wolverine;

namespace Movies.Api.Tests.Features.Movies.Controllers;

[TestFixture]
public class MovieControllerTests
{
    private Mock<IMessageBus> _messageBusMock;
    private Mock<IDocumentSession> _documentSessionMock;
    private Mock<IRepository<Movie>> _movieRepositoryMock;
    private MovieController _sut;

    [SetUp]
    public void Setup()
    {
        _messageBusMock = new Mock<IMessageBus>();
        _documentSessionMock = new Mock<IDocumentSession>();
        _movieRepositoryMock = new Mock<IRepository<Movie>>();
        _sut = new MovieController(_messageBusMock.Object, _documentSessionMock.Object, _movieRepositoryMock.Object);
    }

    [Test]
    public async Task Get_returns_notFound_if_movie_is_not_found()
    {
        var movieDetailsRepositoryMock = new Mock<IRepository<MovieDetails>>();
        // Act
        var result = await _sut.Get(movieDetailsRepositoryMock.Object, Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task Get_returns_movie_details()
    {
        var movie = Builder<Movie>.CreateNew().Build();

        _movieRepositoryMock.Setup(x => x.GetById(movie.Id))
            .ReturnsAsync(movie);

        var movieDetailsRepositoryMock = new Mock<IRepository<MovieDetails>>();

        var movieDetails = Builder<MovieDetails>.CreateNew().Build();
        movieDetailsRepositoryMock.Setup(x => x.GetById(movie.Id))
            .ReturnsAsync(movieDetails);

        // Act
        var result = (await _sut.Get(movieDetailsRepositoryMock.Object, movie.Id) as OkObjectResult);

        // Assert
        result.Value.Should().BeEquivalentTo(movieDetails);
    }

    [Test]
    public async Task List_returns_list_of_movies()
    {
        var listItems = new[]
        {
            Builder<MovieListItem>.CreateNew().Build(),
            Builder<MovieListItem>.CreateNew().Build(),
            Builder<MovieListItem>.CreateNew().Build(),
        };

        var movieListRepositoryMock = new Mock<IRepository<MovieListItem>>();
        movieListRepositoryMock.Setup(x => x.All())
            .ReturnsAsync(listItems);

        // Act
        var result = await _sut.List(movieListRepositoryMock.Object, 1, 10) as OkObjectResult;

        // Assert
        result.Value.Should().BeEquivalentTo(new MovieListModel()
        {
            Items = new[]
            {
                new MovieListItemModel()
                {
                    Id = listItems[0].Id,
                    Title = listItems[0].Title,
                },
                new MovieListItemModel()
                {
                    Id = listItems[1].Id,
                    Title = listItems[1].Title,
                },
                new MovieListItemModel()
                {
                    Id = listItems[2].Id,
                    Title = listItems[2].Title,
                }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 3
        });
    }

    [Test]
    public async Task Create_returns_badRequest_if_validation_fails()
    {
        var requestValidator = new Mock<IValidator<CreateMovieRequest>>();

        var request = Builder<CreateMovieRequest>.CreateNew().Build();

        requestValidator.Setup(x => x.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult
            {
                Errors =
                [
                    new ValidationFailure("Key", "Error"),
                ]
            });

        // Act
        var result = await _sut.Create(requestValidator.Object, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<CreateMovieCommand>(), default, null), Times.Never);
    }

    [Test]
    public async Task Create_returns_badRequest_if_command_fails()
    {
        var requestValidator = new Mock<IValidator<CreateMovieRequest>>();

        var request = Builder<CreateMovieRequest>.CreateNew().Build();
        requestValidator.Setup(x => x.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        _messageBusMock.Setup(x => x.InvokeAsync<Result>(It.IsAny<CreateMovieCommand>(), default, null))
            .ReturnsAsync(Result.Failure("Key", "Error"));

        // Act
        var result = await _sut.Create(requestValidator.Object, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<CreateMovieCommand>(), default, null), Times.Once);
    }

    [Test]
    public async Task Create_returns_ok_if_command_succeeds()
    {
        var requestValidator = new Mock<IValidator<CreateMovieRequest>>();

        var request = Builder<CreateMovieRequest>.CreateNew().Build();
        requestValidator.Setup(x => x.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        _messageBusMock.Setup(x => x.InvokeAsync<Result>(It.IsAny<CreateMovieCommand>(), default, null))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.Create(requestValidator.Object, request);

        // Assert
        result.Should().BeOfType<OkResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<CreateMovieCommand>(), default, null), Times.Once);
    }

    [Test]
    public async Task Rate_returns_badRequest_if_validation_fails()
    {
        var requestValidator = new Mock<IValidator<RateMovieRequest>>();

        var request = Builder<RateMovieRequest>.CreateNew().Build();

        requestValidator.Setup(x => x.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult
            {
                Errors =
                [
                    new ValidationFailure("Key", "Error"),
                ]
            });

        // Act
        var result = await _sut.Rate(requestValidator.Object, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null), Times.Never);
    }

    [Test]
    public async Task Rate_returns_notFound_if_movie_is_not_found()
    {
        var requestValidator = new Mock<IValidator<RateMovieRequest>>();

        var request = Builder<RateMovieRequest>.CreateNew().Build();
        requestValidator.Setup(x => x.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        _movieRepositoryMock.Setup(x => x.GetById(request.Id))
            .ReturnsAsync(null! as Movie);

        // Act
        var result = await _sut.Rate(requestValidator.Object, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null), Times.Never);
    }

    [Test]
    public async Task Rate_returns_badRequest_if_command_fails()
    {
        var requestValidator = new Mock<IValidator<RateMovieRequest>>();

        var request = Builder<RateMovieRequest>.CreateNew().Build();
        requestValidator.Setup(x => x.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        var movie = Builder<Movie>.CreateNew().Build();
        _movieRepositoryMock.Setup(x => x.GetById(request.Id))
            .ReturnsAsync(movie);

        _messageBusMock.Setup(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null))
            .ReturnsAsync(Result.Failure("Key", "Error"));

        // Act
        var result = await _sut.Rate(requestValidator.Object, request);

        // Assert 
        result.Should().BeOfType<BadRequestObjectResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null), Times.Once);
    }

    [Test]
    public async Task Rate_returns_ok_if_command_succeeds()
    {
        var requestValidator = new Mock<IValidator<RateMovieRequest>>();

        var request = Builder<RateMovieRequest>.CreateNew().Build();
        requestValidator.Setup(x => x.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        var movie = Builder<Movie>.CreateNew().Build();
        _movieRepositoryMock.Setup(x => x.GetById(request.Id))
            .ReturnsAsync(movie);

        _messageBusMock.Setup(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.Rate(requestValidator.Object, request);

        // Assert 
        result.Should().BeOfType<OkResult>();
        _messageBusMock.Verify(x => x.InvokeAsync<Result>(It.IsAny<RateMovieCommand>(), default, null), Times.Once);
    }
}