using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Cargo
{
    public class CargoDTO
    {
        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}