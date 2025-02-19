using FluentValidation.TestHelper;
using Movies.Api.Features.Movies.Requests;

namespace Movies.Api.Tests.Features.Movies.Requests;

[TestFixture]
public class RateMovieRequestTests
{
    private RateMovieRequest.Validator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new RateMovieRequest.Validator();
    }

    [Test]
    public void Validator_returns_errors_if_properties_are_null()
    {
        var request = new RateMovieRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Act
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required");
        result.ShouldHaveValidationErrorFor(x => x.Rating).WithErrorMessage("Rating is required");
    }

    [Test]
    public void Validator_returns_error_if_rating_is_below_zero()
    {
        var request = new RateMovieRequest()
        {
            Id = Guid.NewGuid(),
            Rating = -1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating).WithErrorMessage("Rating must be 0 or above");
    }

    [Test]
    public void Validator_returns_error_if_rating_is_above_five()
    {
        var request = new RateMovieRequest()
        {
            Id = Guid.NewGuid(),
            Rating = 6
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating).WithErrorMessage("Rating must be 5 or below");
    }
}