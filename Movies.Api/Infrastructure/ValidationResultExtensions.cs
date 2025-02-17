using FluentValidation.Results;
using Movies.Domain.Features.Movies.Commands;

namespace Movies.Api.Infrastructure;

public static class ValidationResultExtensions
{
    public static Result ToResult(this ValidationResult validationResult)
    {
        if (validationResult.IsValid)
        {
            return Result.Success(); // No errors, return Success result
        }

        var errors = validationResult.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key, 
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return Result.Failure(errors); // Return Failure with errors
    }
}