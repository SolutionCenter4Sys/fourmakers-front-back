using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class PrestadorServicoDTO
    {
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("razaoSocial")]
        [Required(ErrorMessage = "A razão social é obrigatória.")]
        public string RazaoSocial { get; set; }

        [JsonPropertyName("nomeFantasia")]
        [Required(ErrorMessage = "O nome fantasia é obrigatório.")]
        public string NomeFantasia { get; set; }
        public string ContaDigito { get; set; }
        [JsonPropertyName("agencia")]
        public string Agencia { get; set; }

        [JsonPropertyName("codigoBanco")]
        [Required(ErrorMessage = "O código do banco é obrigatório.")]
        public string CodigoBanco { get; set; }
        //    [JsonPropertyName("regimeTributario")]
        //    [Required(ErrorMessage = "O Regime tributário é obrigatório.")]
        //    //public RegimeTributarioDTO RegimeTributario { get; set;}
    }
}