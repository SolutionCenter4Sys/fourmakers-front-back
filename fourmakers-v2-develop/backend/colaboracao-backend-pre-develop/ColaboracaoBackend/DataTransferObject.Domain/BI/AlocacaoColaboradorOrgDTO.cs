using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain
{
    public class AlocacaoColaboradorOrgDTO
    {
        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("orgNome")]
        public string OrgNome { get; set; }
        [JsonPropertyName("alocacoes")]
        public List<dynamic> Alocacoes { get; set; }
    }
}