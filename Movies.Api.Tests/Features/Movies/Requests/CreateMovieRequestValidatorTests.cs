using FluentValidation.TestHelper;
using Movies.Api.Features.Movies.Requests;

namespace Movies.Api.Tests.Features.Movies.Requests;

[TestFixture]
public class CreateMovieRequestValidatorTests
{
    private CreateMovieRequest.Validator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateMovieRequest.Validator();
    }

    [Test]
    public void Validator_returns_errors_when_properties_are_null()
    {
        var request = new CreateMovieRequest();
        
        // Act
        var result = _validator.TestValidate(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title).WithErrorMessage("Title is required");
        result.ShouldHaveValidationErrorFor(x => x.Overview).WithErrorMessage("Overview is required");
    }
}