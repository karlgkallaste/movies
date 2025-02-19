using FluentValidation;
using Movies.Domain.Features.Movies;

namespace Movies.Api.Features.Movies.Requests;

public class CreateMovieRequest
{
    
    public string Title { get; set; } = null!;
    public string Overview { get; set; } = null!;
    public MovieStatus Status { get; set; }
    
    public class Validator : AbstractValidator<CreateMovieRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotNull().WithMessage("Title is required");
            RuleFor(x => x.Overview).NotNull().WithMessage("Overview is required");
        
        }
    }
}