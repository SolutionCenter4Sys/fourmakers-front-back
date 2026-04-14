using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Util;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class ColaboradoresOrgIdResult : StatusResult
    {
        public ColaboradoresOrgIdResult()
        {
        }

        [JsonPropertyName("id")]
        public long? Id_usuario { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("nome_completo")]
        public string Nome { get; set; }

        [JsonPropertyName("org")]
        public OrgDTO Org { get; set; }

        [JsonPropertyName("imagemPath")]
        public string ImagemPath { get; set; }

        [JsonPropertyName("data_criacao")]
        public string DataCadastro { get; set; }

        public string Status { get; set; }
    }
}