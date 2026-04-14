using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ProjetoGerenteResult
    {
        [JsonPropertyName("codProjeto")]
        public string cod_projeto { get; set; }

        [JsonPropertyName("nomeProjeto")]
        public string projeto { get; set; }
        [JsonIgnore]
        public bool permite_apont_sem_alocacao { get; set; }
    }
}