using System.Text.Json.Serialization;

namespace NewApiAppColaboracao.Models.BI
{
    public class InfoEndossoGrafico
    {
        [JsonPropertyName("naoSei")]
        public int NaoSei { get; set; }
        [JsonPropertyName("nada")]
        public int Nada { get; set; }
        [JsonPropertyName("pouco")]
        public int Pouco { get; set; }
        [JsonPropertyName("regular")]
        public int Regular { get; set; }
        [JsonPropertyName("bastante")]
        public int Bastante { get; set; }
        [JsonPropertyName("domina")]
        public int Domina { get; set; }
        [JsonPropertyName("semEndosso")]
        public int SemEndosso { get; set; }
    }
}