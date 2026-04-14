using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class RetornoColaboradorBIDTO
    {
        [Required]
        [Display(Name = "CPF")]
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("flagCandidato")]
        public bool FlagCandidato { get; set; }

        [JsonPropertyName("flagAtivo")]
        public bool FlagAtivo { get; set; }

        [JsonPropertyName("perfilProfissional")]
        public PerfilProfissionalRetornoBIDTO PerfilProfissional { get; set; }
    }
}