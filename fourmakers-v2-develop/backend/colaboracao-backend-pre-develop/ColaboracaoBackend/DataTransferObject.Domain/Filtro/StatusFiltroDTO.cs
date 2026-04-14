using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class StatusFiltroDTO
    {
        [JsonPropertyName("ativo")]
        public int Ativo { get; set; }

        [JsonPropertyName("inativo")]
        public int Inativo { get; set; }
        public string CpfColaborador { get; set; }
        public string ColaboradorNome { get; set; }
    }
}