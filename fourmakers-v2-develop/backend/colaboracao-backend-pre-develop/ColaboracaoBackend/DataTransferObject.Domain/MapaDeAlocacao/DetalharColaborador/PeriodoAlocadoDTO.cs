using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class PeriodoAlocadoDTO : PeriodoDTO
    {
        [JsonPropertyName("cpfColaborador")]
        public string CpfColaborador { get; set; }
        [JsonPropertyName("codigoTBD")]
        public int? CodigoTBD { get; set; }
        [JsonPropertyName("nomeProjeto")]
        public string NomeProjeto { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string LabelProjetoCliente { get; set; }
        [JsonPropertyName("codigoProjeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("nomeGestor")]
        public string NomeGestor { get; set; }
        [JsonPropertyName("codigoGestor")]
        public string CodigoGestor { get; set; }
    }
}