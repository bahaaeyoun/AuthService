using System.Text.Json.Serialization;

namespace Fathy.Common.Startup;

public class Error(int statusCode, string errorCode, string description)
{
    [JsonIgnore] public int StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
    public string Description { get; } = description;

    public Error(string errorCode, string description) : this(0, errorCode, description)
    {
    }
}