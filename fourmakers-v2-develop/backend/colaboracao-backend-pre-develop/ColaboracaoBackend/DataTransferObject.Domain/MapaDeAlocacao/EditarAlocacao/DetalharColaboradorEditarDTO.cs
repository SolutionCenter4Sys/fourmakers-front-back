using DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class DetalharColaboradorEditarDTO
    {
        [JsonPropertyName("nomeColaborador")]
        public string Nome { get; set; }
        [JsonPropertyName("codigoColaborador")]
        public string CodigoColaborador { get; set; }
        [JsonPropertyName("cpfColaborador")]
        public string CpfColaborador { get; set; }
        [JsonPropertyName("codigoTBD")]
        public int? CodigoTBD { get; set; }
        [JsonPropertyName("periodo")]
        public string periodo { get; set; }
        [JsonPropertyName("projetos")]
        public List<ProjetosDetalhadosEditarDTO> Projetos { get; set; }
    }
}