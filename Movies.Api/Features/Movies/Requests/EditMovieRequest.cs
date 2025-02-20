using FluentValidation;

namespace Movies.Api.Features.Movies.Requests;

public class EditMovieRequest
{
    public Guid Id { get; set; }
    public string Overview { get; set; } = null!;
    public decimal Budget { get; set; }
    public string? HomePage { get; set; }

    public class Validator : AbstractValidator<EditMovieRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.Overview).NotNull().WithMessage("Overview is required");
            RuleFor(x => x.HomePage).Matches("([\\w\\-]+(\\.[\\w\\-]+)+)")
                .WithMessage("Invalid homepage url").When(x => !string.IsNullOrWhiteSpace(x.HomePage));
        }
    }
}