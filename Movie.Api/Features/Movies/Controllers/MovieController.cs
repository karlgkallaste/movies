using Marten;
using Microsoft.AspNetCore.Mvc;
using Movie.Api.Features.Movies.Models;
using Movie.Domain.Features.Movies.Commands;
using Movie.Domain.Features.Movies.Projections;
using Wolverine;

namespace Movie.Api.Features.Movies.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 400)]
    public async Task<IActionResult> Get([FromServices] IMessageBus bus)
    {
        await bus.InvokeAsync(new CreateMovieCommand(Guid.NewGuid()));
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

        int totalCount = await query.CountAsync(); // Total movies count

        var movies = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = movies.Select(m => new MovieListItemModel()
        {
            Id = m.Id,
        }).ToList();

        var result = new MovieListModel
        {
            Items = dtoList,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Ok(result);
    }
}