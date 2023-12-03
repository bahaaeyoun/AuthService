using Newtonsoft.Json;

namespace Fathy.Common.Startup;

public static class Extensions
{
    public static string ToJsonStringContent<T>(this T data) => JsonConvert.SerializeObject(data);
}