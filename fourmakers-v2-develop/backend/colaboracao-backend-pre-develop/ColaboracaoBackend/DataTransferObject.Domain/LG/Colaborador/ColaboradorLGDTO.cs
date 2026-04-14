using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG.Colaborador
{
    public class ColaboradorLGDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("org_id")]
        public long OrgId { get; set; }

        [JsonPropertyName("lg_pessoa_id")]
        public long PessoaId { get; set; }

        [JsonPropertyName("matricula")]
        public string Matricula { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("codigo_empresa")]
        public int CodigoEmpresa { get; set; }
    }
}