using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ProjetosColaboradorDTO
    {
        [JsonPropertyName("projetos")]
        public string NomeProjeto { get; set; }
        [JsonPropertyName("codigoProjeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("codigoCliente")]
        public string CodigoCliente { get; set; }
    }
}