using System.Net;

namespace VZaps;

public class VZapsException : Exception
{
    public VZapsException(string message)
        : base(message)
    {
    }

    public VZapsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class VZapsApiException : VZapsException
{
    public VZapsApiException(
        string message,
        HttpStatusCode statusCode,
        string? errorCode = null,
        string? details = null,
        string? requestId = null,
        string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Details = details;
        RequestId = requestId;
        ResponseBody = responseBody;
    }

    public HttpStatusCode StatusCode { get; }

    public string? ErrorCode { get; }

    public string? Details { get; }

    public string? RequestId { get; }

    public string? ResponseBody { get; }
}

public sealed class VZapsAuthenticationException : VZapsApiException
{
    public VZapsAuthenticationException(
        string message,
        HttpStatusCode statusCode,
        string? errorCode = null,
        string? details = null,
        string? requestId = null,
        string? responseBody = null)
        : base(message, statusCode, errorCode, details, requestId, responseBody)
    {
    }
}

public sealed class VZapsRateLimitException : VZapsApiException
{
    public VZapsRateLimitException(
        string message,
        HttpStatusCode statusCode,
        string? errorCode = null,
        string? details = null,
        string? requestId = null,
        string? responseBody = null)
        : base(message, statusCode, errorCode, details, requestId, responseBody)
    {
    }
}

public sealed class VZapsTimeoutException : VZapsException
{
    public VZapsTimeoutException(string message = "The VZaps request timed out.")
        : base(message)
    {
    }

    public VZapsTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class VZapsRealtimeException : VZapsException
{
    public VZapsRealtimeException(string message)
        : base(message)
    {
    }

    public VZapsRealtimeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
