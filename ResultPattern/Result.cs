using System.Collections.Immutable;
using System.Net;

namespace ROP;

/// <summary>
/// Provides extension methods to create Result objects.
/// </summary>
public static partial class Result
{
    /// <summary>
    /// Object to avoid using void
    /// </summary>
    public static readonly Unit Unit = Unit.Value;

    /// <summary>
    /// Chains an object into the Result Structure
    /// </summary>
    public static Result<T> Success<T>(this T value) => new(value, HttpStatusCode.OK);

    /// <summary>
    /// Chains an object into the Result Structure
    /// </summary>
    public static Result<T> Success<T>(this T value, HttpStatusCode httpStatusCode) => new(value, httpStatusCode);

    /// <summary>
    /// Chains an Result.Unit into the Result Structure
    /// </summary>
    public static Result<Unit> Success() => new(Unit, HttpStatusCode.OK);

    /// <summary>
    /// Converts the type into the error flow with  HttpStatusCode.BadRequest
    /// </summary>
    public static Result<T> Failure<T>(ImmutableArray<Error> errors) => new(errors, HttpStatusCode.BadRequest);

    /// <summary>
    /// Converts the type into the error flow with  HttpStatusCode.BadRequest
    /// </summary>
    public static Result<T> Failure<T>(ImmutableArray<Error> errors, HttpStatusCode httpStatusCode) => new(errors, httpStatusCode);

    /// <summary>
    /// Converts the type into the error flow with  HttpStatusCode.BadRequest
    /// </summary>
    public static Result<T> Failure<T>(Error error) => new([error], HttpStatusCode.BadRequest);


    /// <summary>
    /// Converts a synchronous Result structure into async
    /// </summary>
    public static Task<Result<T>> Async<T>(this Result<T> r) => Task.FromResult(r);
}