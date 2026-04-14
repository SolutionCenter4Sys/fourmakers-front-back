using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Cargo
{
    public class CargoResult : StatusResult
    {
        [JsonPropertyName("cargo")]
        public CargoDTO Cargo { get; set; }
    }
}