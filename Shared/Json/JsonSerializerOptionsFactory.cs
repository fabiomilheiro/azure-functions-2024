using System.Text.Json;
using System.Text.Json.Serialization;

namespace Azf.Shared.Json
{
    public static class JsonSerializerOptionsFactory
    {
        private static JsonSerializerOptions? options;

        public static JsonSerializerOptions GetDefault(AppSettings settings)
        {
            if (options != null)
            {
                return options;
            }

            options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            };

            if (settings.Environment == AppEnvironment.Production)
            {
                options.WriteIndented = false;
            }

            return options;
        }
    }
}