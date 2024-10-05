using System.Text.Json;

namespace Azf.Shared.Json
{
    public interface IJsonService
    {
        string Serialize(object source);

        TTarget? Deserialize<TTarget>(string source);

        object? Deserialize(string source, Type targetType);
    }

    public class JsonService : IJsonService
    {
        private readonly JsonSerializerOptions options;

        public JsonService(JsonSerializerOptions options)
        {
            this.options = options;
        }

        public string Serialize(object source)
        {
            return JsonSerializer.Serialize(source, options);
        }

        public TTarget? Deserialize<TTarget>(string source)
        {
            return JsonSerializer.Deserialize<TTarget>(source, options);
        }

        public object? Deserialize(string source, Type targetType)
        {
            return JsonSerializer.Deserialize(source, targetType, options);
        }
    }
}
