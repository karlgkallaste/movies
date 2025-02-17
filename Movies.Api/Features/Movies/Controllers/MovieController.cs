using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Infrastructure;
using Movies.Domain.Features.Movies.Commands;
using Movies.Domain.Features.Movies.Projections;
using Movies.Api.Features.Movies.Models;
using Movies.Api.Features.Movies.Requests;
using Wolverine;

namespace Movies.Api.Features.Movies.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    public async Task<IActionResult> Get([FromServices] IDocumentSession documentSession)
    {
        var query = documentSession.Query<MovieDetails>();
        return Ok();
    }

    [HttpGet("list")]
    [ProducesResponseType(typeof(MovieListModel), 200)]
    [ProducesResponseType(typeof(void), 400)]
    public async Task<IActionResult> List([FromServices] IDocumentSession documentSession, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = documentSession.Query<MovieDetails>();

        int totalCount = await query.CountAsync();

        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new MovieListModel
        {
            Items = movies.Select(m => new MovieListItemModel()
            {
                Id = m.Id,
                Title = m.Title
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Ok(result);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(Result), 400)]
    public async Task<IActionResult> Create([FromServices] IValidator<CreateMovieRequest> validator, [FromServices] IMessageBus messageBus, [FromBody] CreateMovieRequest request)
    {
        var validationResult = (await validator.ValidateAsync(request)).ToResult();

        if (!validationResult.IsSuccess)
        {
            return BadRequest(validationResult); 
        }
        
        var commandResult = await messageBus.InvokeAsync<Result>(new CreateMovieCommand(request.Title, request.Overview));

        if (!commandResult.IsSuccess)
        {
            return BadRequest(Result.Failure("cmd","Failure"));
        }
        return Ok();
    }
}