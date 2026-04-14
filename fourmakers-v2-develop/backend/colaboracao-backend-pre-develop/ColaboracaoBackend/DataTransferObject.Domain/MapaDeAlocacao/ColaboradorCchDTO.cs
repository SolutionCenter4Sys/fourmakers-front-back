using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ColaboradorCchDTO
    {
        [JsonPropertyName("cd_Profissional")]
        public string CdProfissional { get; set; }

        [JsonPropertyName("nm_Profissional")]
        public string NmProfissional { get; set; }
        
        [JsonPropertyName("codigoColaboradorInterno")]
        public string CodigoColaboradorInterno { get; set; }
    }
}