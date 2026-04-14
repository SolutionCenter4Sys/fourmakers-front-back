using System;
using System.Text.Json;

namespace Colaboracao.Helper.Util
{
    public static class JsonUtil
    {
        public static T TryDeserializeJsonNewtonsoft<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return default;
            }
        }
    }
}