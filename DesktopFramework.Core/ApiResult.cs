namespace DesktopFramework.Core;

/// <summary>
/// Outcome of an API call. Failures are returned as data (not thrown) so the UI
/// can render a friendly error in-place instead of crashing the front end.
/// </summary>
public sealed class ApiResult<T>
{
    private ApiResult(bool isSuccess, T? value, int? statusCode, string? errorTitle, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        StatusCode = statusCode;
        ErrorTitle = errorTitle;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }

    /// <summary>HTTP status code when the failure came from a response; null for
    /// network errors / timeouts.</summary>
    public int? StatusCode { get; }

    public string? ErrorTitle { get; }
    public string? ErrorMessage { get; }

    public static ApiResult<T> Ok(T value) => new(true, value, null, null, null);

    public static ApiResult<T> Fail(int? statusCode, string title, string message) =>
        new(false, default, statusCode, title, message);

    /// <summary>Re-shapes the value type, preserving any error.</summary>
    public ApiResult<TNew> Cast<TNew>(Func<T, TNew> map) =>
        IsSuccess
            ? ApiResult<TNew>.Ok(map(Value!))
            : ApiResult<TNew>.Fail(StatusCode, ErrorTitle!, ErrorMessage!);
}
