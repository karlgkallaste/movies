using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Infrastructure;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Projections;
using Movies.Api.Features.Movies.Models;
using Movies.Api.Features.Movies.Requests;
using Movies.Data;
using Movies.Domain.Features.Movies;
using Wolverine;

namespace Movies.Api.Features.Movies.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController(
    IMessageBus messageBus,
    IEntityRepository<Movie> movieEntityRepository)
    : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieDetailsModel), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Get(
        [FromServices] IProjectionRepository<MovieDetails> movieDetailsEntityRepository,
        [FromRoute] Guid id)
    {
        var movie = await movieEntityRepository.GetById(id);
        if (movie is null)
        {
            return NotFound();
        }

        var details = await movieDetailsEntityRepository.GetById(movie.Id);
        return Ok(new MovieDetailsModel(details));
    }

    [HttpGet("list")]
    [ProducesResponseType(typeof(MovieListModel), 200)]
    public async Task<IActionResult> List([FromServices] IProjectionRepository<MovieListItem> movieListEntityRepository,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        // Retrieve paged results
        var movies = await movieListEntityRepository.GetPagedResults(page, pageSize);

        var result = new MovieListModel
        {
            Items = movies.Select(m => new MovieListItemModel()
            {
                Id = m.Id,
                Title = m.Title
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = movies.Count
        };

        return Ok(result);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    public async Task<IActionResult> Create([FromServices] IValidator<CreateMovieRequest> validator,
        [FromBody] CreateMovieRequest request)
    {
        var validationResult = (await validator.ValidateAsync(request)).ToResult();

        if (!validationResult.IsSuccess)
        {
            return BadRequest(validationResult);
        }

        var commandResult = await messageBus.InvokeAsync<Result>(new CreateMovieCommand(request.Title, request.Overview,
            request.ReleaseDate, request.Status, request.Genre));

        if (!commandResult.IsSuccess)
        {
            return BadRequest(Result.Failure(nameof(Movie), "Failed to create the movie."));
        }

        return Ok();
    }

    [HttpPost("edit")]
    [ProducesResponseType(typeof(OkResult), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    public async Task<IActionResult> Edit(
        [FromServices] IValidator<EditMovieRequest> validator,
        [FromBody] EditMovieRequest request)
    {
        var validationResult = (await validator.ValidateAsync(request)).ToResult();

        if (!validationResult.IsSuccess)
        {
            return BadRequest(validationResult);
        }

        var movie = await movieEntityRepository.GetById(request.Id);

        if (movie is null)
        {
            return NotFound();
        }

        var commandResult = await messageBus.InvokeAsync<Result>(
            new EditMovieCommand(movie.Id, request.Overview, request.Budget, request.HomePage));

        if (!commandResult.IsSuccess)
        {
            return BadRequest(Result.Failure(nameof(Movie), "Failed to edit movie."));
        }

        return Ok();
    }

    [HttpPost("rate")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
    public async Task<IActionResult> Rate([FromServices] IValidator<RateMovieRequest> validator,
        [FromBody] RateMovieRequest request)
    {
        var validationResult = (await validator.ValidateAsync(request)).ToResult();

        if (!validationResult.IsSuccess)
        {
            return BadRequest(validationResult);
        }

        var movie = await movieEntityRepository.GetById(request.Id);

        if (movie is null)
        {
            return NotFound();
        }

        var commandResult = await messageBus.InvokeAsync<Result>(new RateMovieCommand(request.Id, request.Rating));

        if (!commandResult.IsSuccess)
        {
            return BadRequest(Result.Failure(nameof(Movie), "Failed to rate the movie."));
        }

        return Ok();
    }
}