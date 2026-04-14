using DataTransferObject.Domain.Colaborador;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class EmpresaDTO
    {
        [JsonPropertyName("nomeFantasia")]
        public string NomeFantasia { get; set; }
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }
        [JsonPropertyName("razaoSocial")]
        public string RazaoSocial { get; set; }
        [JsonPropertyName("site")]
        public string Site { get; set; }
        [JsonPropertyName("linkedin")]
        public string Linkedin { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("usuarioResponsavel")]
        public ColaboradorDTO UsuarioResponsavel { get; set; }
    }
}