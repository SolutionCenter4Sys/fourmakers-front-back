using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.CCH
{
    public class HierarquiaResult
    {
        [JsonPropertyName("cdProfissional")]
        public long CodigoProfissional { get; set; }

        [JsonPropertyName("nmProfissional")]
        public string NomeProfissional { get; set; }

        [JsonPropertyName("cdCargo")]
        public long CodigoCargo { get; set; }

        [JsonPropertyName("nmCargo")]
        public string NomeCargo { get; set; }

        [JsonPropertyName("recursos")]
        public List<HierarquiaRecurso> Recurso { get; set; }
    }
}