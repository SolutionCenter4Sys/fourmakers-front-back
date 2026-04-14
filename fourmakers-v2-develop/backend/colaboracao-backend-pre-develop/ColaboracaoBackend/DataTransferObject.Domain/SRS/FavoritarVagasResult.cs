using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class FavoritarVagasResult
    {
        public FavoritarVagasResult()
        {
            VagasFavoritadas = new List<FavoritarVagasDTO>();
        }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sucess")]
        public bool Sucess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("vagasFavoritadas")]
        public List<FavoritarVagasDTO> VagasFavoritadas { get; set; }
    }
}