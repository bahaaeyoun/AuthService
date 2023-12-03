namespace Fathy.Common.Startup;

public class Result
{
    public bool Succeeded { get; }
    public IEnumerable<Error>? Errors { get; }

    public Result(bool succeeded, IEnumerable<Error>? errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public static Result Success() => new(true, default);
    public static Result Failure(IEnumerable<Error>? errors = default) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    public Result(bool succeeded, IEnumerable<Error>? errors, T? data) : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T? data = default) => new(true, default, data);

    public static Result<T> Failure(IEnumerable<Error>? errors = default, T? data = default) =>
        new(false, errors, data);
}