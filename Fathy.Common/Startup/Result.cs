namespace Fathy.Common.Startup;

public class Result(bool succeeded, IEnumerable<Error>? errors)
{
    public bool Succeeded { get; } = succeeded;
    public IEnumerable<Error>? Errors { get; } = errors;

    public static Result Success() => new(true, default);
    public static Result Failure(IEnumerable<Error>? errors = default) => new(false, errors);
}

public class Result<T>(bool succeeded, IEnumerable<Error>? errors, T? data) : Result(succeeded, errors)
{
    public T? Data { get; } = data;

    public static Result<T> Success(T? data = default) => new(true, default, data);

    public static Result<T> Failure(IEnumerable<Error>? errors = default, T? data = default) =>
        new(false, errors, data);
}