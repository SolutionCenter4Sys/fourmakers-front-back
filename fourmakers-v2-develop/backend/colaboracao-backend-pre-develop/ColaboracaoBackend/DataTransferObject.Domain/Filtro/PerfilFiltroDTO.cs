using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class PerfilFiltroDTO
    {
        [JsonPropertyName("colaborador")]
        public int Colaborador { get; set; }

        [JsonPropertyName("candidato")]
        public int Candidato { get; set; }

        public string CpfColaborador { get; set; }

        public string ColaboradorNome { get; set; }
    }
}