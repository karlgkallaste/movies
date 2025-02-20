using FluentValidation.TestHelper;
using Movies.Api.Features.Movies.Requests;

namespace Movies.Api.Tests.Features.Movies.Requests;

[TestFixture]
public class EditMovieRequestValidatorTests
{
    private EditMovieRequest.Validator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new EditMovieRequest.Validator();
    }

    [Test]
    public void Validator_returns_errors_when_properties_are_null()
    {
        var request = new EditMovieRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id is required");
        result.ShouldHaveValidationErrorFor(x => x.Overview).WithErrorMessage("Overview is required");
    }

    [TestCase("xd,dx", false)]
    [TestCase("test.ee", true)]
    public void Validator_returns_errors_when_homepage_url_is_invalid(string url, bool isValid)
    {
        var request = new EditMovieRequest()
        {
            HomePage = url
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        if (!isValid)
        {
            result.ShouldHaveValidationErrorFor(x => x.HomePage)
                .WithErrorMessage("Invalid homepage url");
        }

        else
        {
            result.ShouldNotHaveValidationErrorFor(x => x.HomePage);
        }
    }
}