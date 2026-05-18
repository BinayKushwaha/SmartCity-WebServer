using System.Text.Json.Serialization;

namespace SmartCity.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertyType
    {
        Residential = 1,
        Commercial = 2,
        Industrial = 3,
        Agricultural = 4
    }
}
