using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao
{
    public class ConflitoAlocacao
    {
        [JsonPropertyName("codigoColaborador")]
        public string CodigoColaborador { get; set; }

        [JsonPropertyName("CpfColaborador")]
        public string CpfColaborador { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("qtdConflitoAlocacao")]
        public int QtdConflitoAlocacao { get; set; }
    }
}