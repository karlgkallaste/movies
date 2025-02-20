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
    /// <summary>
    /// Retrieves movie details by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the movie.</param>
    /// <returns>
    /// Returns the details of the movie if found; otherwise, returns a 404 Not Found response.
    /// </returns>
    /// <response code="200">Returns the movie details.</response>
    /// <response code="404">If the movie is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieDetailsModel), 200)]
    [ProducesResponseType(typeof(NotFoundResult), 404)]
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

    /// <summary>
    /// Retrieves a paginated list of movies.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>
    /// Returns a paginated list of movies.
    /// </returns>
    /// <response code="200">Returns the paginated list of movies.</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(MovieListModel), 200)]
    public async Task<IActionResult> List([FromServices] IProjectionRepository<MovieListItem> movieListEntityRepository,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var movies = await movieListEntityRepository.GetPagedResults(page, pageSize);

        var result = new MovieListModel
        {
            Items = movies.Select(m => new MovieListItemModel()
            {
                Id = m.Id,
                Title = m.Title,
                Overview = m.Overview,
                Popularity = m.Popularity,
                ReleaseDate = m.ReleaseDate
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = movies.Count
        };

        return Ok(result);
    }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="request">The request model containing movie details.</param>
    /// <returns>
    /// Returns an HTTP 200 response if the operation is successful, 
    /// or an HTTP 400 response if the request is invalid.
    /// </returns>
    /// <response code="200">Movie created or updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(void), 200)]
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

    /// <summary>
    /// Edits an existing movie's details.
    /// </summary>
    /// <param name="request">The request model containing the updated movie details.</param>
    /// <returns>
    /// Returns an HTTP 200 response if the movie was successfully updated, 
    /// or an HTTP 400 response if the request is invalid.
    /// </returns>
    /// <response code="200">Movie updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    [HttpPost("edit")]
    [ProducesResponseType(typeof(void), 200)]
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

    /// <summary>
    /// Rates a movie by adding a user rating.
    /// </summary>
    /// <param name="request">The request model containing the movie ID and rating value.</param>
    /// <returns>
    /// Returns an HTTP 200 response if the rating was successfully added, 
    /// an HTTP 400 response if the request is invalid, or 
    /// an HTTP 404 response if the movie is not found.
    /// </returns>
    /// <response code="200">Rating added successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="404">Movie not found.</response>
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