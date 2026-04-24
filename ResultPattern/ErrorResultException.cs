using System.Collections.Immutable;

namespace ROP;

///<summary>
/// Custom exception that encapsulates the errors
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ErrorResultException"/> class.
/// </remarks>
/// <param name="errors"></param>
public class ErrorResultException(ImmutableArray<Error> errors) : Exception(ValidateAndGetErrorMessage(errors))
{
    /// <summary>
    /// The errors that occurred.
    /// </summary>
    public ImmutableArray<Error> Errors { get; } = errors;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResultException"/> class.
    /// </summary>
    /// <param name="error"></param>
    public ErrorResultException(Error error)
        : this([error])
    {
    }

    private static string ValidateAndGetErrorMessage(ImmutableArray<Error> errors)
    {
        if (errors.Length == 0)
            throw new Exception("You should include at least one Error");

        if (errors.Length == 1)
            return errors[0].Message;

        return errors
            .Select(e => e.Message)
            .Prepend($"{errors.Length} Errors occurred:")
            .JoinStrings(Environment.NewLine);
    }
}