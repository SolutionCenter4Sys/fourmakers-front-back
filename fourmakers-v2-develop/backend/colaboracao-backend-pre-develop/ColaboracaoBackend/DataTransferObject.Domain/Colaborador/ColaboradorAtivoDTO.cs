using DataTransferObject.Domain.Diretoria;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorAtivoDTO
    {
        public ColaboradorAtivoDTO()
        {
            Diretoria = new DiretoriaDTO();
            Status = new StatusColaboradorResult();
        }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("diretoria")]
        public DiretoriaDTO Diretoria { get; set; }

        [JsonPropertyName("status")]
        public StatusColaboradorResult Status { get; set; }

        [JsonPropertyName("slack_id")]
        public string Slack_id { get; set; }
    }
}