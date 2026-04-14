using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class MergeItemPerfilParam
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("itens")]
        public List<AdicionarRemoverItemParam> Itens { get; set; }
    }
}