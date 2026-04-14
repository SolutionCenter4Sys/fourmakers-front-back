using DataTransferObject.Domain.Org;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class ListaOrgColaboradorDTO
    {
        [JsonPropertyName("colaboradorOrg")]
        public ColaboradorOrgDTO ColaboradorOrg { get; set; }
        [JsonPropertyName("hierarquiaOrg")]
        public ColaboradorOrgHierarquiaDTO HierarquiaOrg { get; set; }
    }
}