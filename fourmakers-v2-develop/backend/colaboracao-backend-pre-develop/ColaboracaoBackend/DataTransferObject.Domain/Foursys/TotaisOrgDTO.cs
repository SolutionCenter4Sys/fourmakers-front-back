using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class TotaisOrgDTO
    {
        [JsonPropertyName("orgId")]
        public long OrgId { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}