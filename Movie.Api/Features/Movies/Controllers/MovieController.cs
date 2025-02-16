using Microsoft.AspNetCore.Mvc;
using Movie.Domain.Features.Movies.Commands;
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
}