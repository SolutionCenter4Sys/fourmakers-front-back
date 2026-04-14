using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Certificado
{
    public class ColaboradorCompetenciaCertificadoDTO
    {
        [JsonPropertyName("ativo")]
        public sbyte Ativo { get; set; }

        [JsonPropertyName("cpfTbColaboradroCompetencia")]
        public string CpfTbColaboradroCompetencia { get; set; }

        [JsonPropertyName("principal")]
        public sbyte Principal { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}