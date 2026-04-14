using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Org
{
    public class ColaboradorOrgHierarquiaDTO
    {
        [JsonPropertyName("codigoProfissionalSuperior")]
        public string CodProfissionalSuperior { get; set; }
        [JsonPropertyName("nomeProfissionalSuperior")]
        public string NomeProfissionalSuperior { get; set; }
        [JsonPropertyName("codigoInternoProfissionalSuperior")]
        public string CodigoInternoProfissionalSuperior { get; set; }
        [JsonPropertyName("emailProfissionalSuperior")]
        public string EmailProfissionalSuperior { get; set; }
    }
}