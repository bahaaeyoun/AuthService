using System.Text.Json.Serialization;

namespace Fathy.Common.Startup;

public class Error
{
    [JsonIgnore] public int StatusCode { get; }
    public string ErrorCode { get; }
    public string Description { get; }

    public Error(string errorCode, string description)
    {
        ErrorCode = errorCode;
        Description = description;
    }

    public Error(int statusCode, string errorCode, string description)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Description = description;
    }
}