using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.Projetos
{
    public class BuscaProjetoHorasDTO
    {
        [JsonPropertyName("nomeProjeto")]
        public string NomeProjeto { get; set; }
        [JsonPropertyName("codigoProjeto")]
        public long CodigoProjeto { get; set; }
    }
}