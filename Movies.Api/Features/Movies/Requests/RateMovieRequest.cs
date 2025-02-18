using FluentValidation;

namespace Movies.Api.Features.Movies.Requests;

public class RateMovieRequest
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    
    public class Validator : AbstractValidator<RateMovieRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Title is required");
            RuleFor(x => x.Rating)
                .Must(x => x >= 0).WithMessage("Rating must be 0 or above")
                .Must(x => x <= 5).WithMessage("Rating must be 5 or below")
                .NotNull().WithMessage("Overview is required");
        
        }
    }
}
