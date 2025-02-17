namespace Movies.Domain.Features.Movies.Commands;

public class Result
{
    public bool IsSuccess { get; }
    public Dictionary<string, string[]>? Errors { get; }

    // Success constructor (no errors)
    private Result()
    {
        IsSuccess = true;
        Errors = null;
    }

    // Failure constructor (multiple errors as a dictionary)
    private Result(Dictionary<string, string[]> errors)
    {
        IsSuccess = false;
        Errors = errors ?? throw new ArgumentNullException(nameof(errors), "Errors cannot be null.");
    }

    // Failure constructor (single error with key and message)
    private Result(string key, string message)
    {
        IsSuccess = false;
        Errors = new Dictionary<string, string[]>
        {
            { key, new[] { message } }
        };
    }

    // Static methods for success and failure
    public static Result Success() => new Result();

    // Failure with multiple errors (Dictionary<string, string[]>)
    public static Result Failure(Dictionary<string, string[]> errors) => new(errors);

    // Failure with a single error (key, message)
    public static Result Failure(string key, string message) => new(key, message);
}
