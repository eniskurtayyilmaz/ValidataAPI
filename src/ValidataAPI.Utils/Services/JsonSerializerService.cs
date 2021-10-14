using System.Text.Json;

namespace ValidataAPI.Utils.Services
{
    public interface IJsonSerializerService
    {
        string Serialize(object value);
    }

    public class JsonSerializerService : IJsonSerializerService
    {
        private readonly JsonSerializerOptions _serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, _serializeOptions);
        }
    }
}