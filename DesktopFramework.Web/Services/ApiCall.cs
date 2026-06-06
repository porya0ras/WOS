using DesktopFramework.Core;

namespace DesktopFramework.Web.Services;

/// <summary>
/// Shared wrapper that turns HTTP errors, timeouts and network failures into an
/// <see cref="ApiResult{T}"/> with a friendly message — so callers never throw.
/// </summary>
public static class ApiCall
{
    public static async Task<ApiResult<T>> SendAsync<T>(Func<CancellationToken, Task<T?>> call, CancellationToken ct)
    {
        try
        {
            var value = await call(ct);
            return value is not null
                ? ApiResult<T>.Ok(value)
                : ApiResult<T>.Fail(null, "No data", "The server returned an empty response.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode is { } status)
        {
            var (title, message) = Describe((int)status);
            return ApiResult<T>.Fail((int)status, title, message);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Fail(null, "Can't reach the server",
                "The API isn't responding. Check your connection and try again.");
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw; // the caller cancelled deliberately — not an error to display
        }
        catch (Exception)
        {
            // Timeouts (resilience handler), JSON errors, etc.
            return ApiResult<T>.Fail(null, "Request failed",
                "The request timed out or could not be completed. Please try again.");
        }
    }

    private static (string Title, string Message) Describe(int status) => status switch
    {
        400 => ("Bad request", "The request was invalid. Please try again."),
        401 => ("Not signed in", "You need to sign in to view this."),
        403 => ("Access denied", "You don't have permission to view this."),
        404 => ("Not found", "The requested content was not found."),
        408 => ("Timed out", "The request timed out. Please try again."),
        429 => ("Too many requests", "Please wait a moment and try again."),
        >= 500 and < 600 => ("Server error", "The server ran into a problem. Please try again shortly."),
        _ => ("Something went wrong", "Couldn't load data. Please try again."),
    };
}
