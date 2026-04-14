using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class EditarCompetenciaDTO
    {
        [JsonPropertyName("idCompetencia")]
        public long Id { get; set; }
        [JsonPropertyName("descricaoCompetencia")]
        public string Descricao { get; set; }
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }
    }
}