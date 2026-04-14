using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class ProjetosDetalhadosDTO
    {
        public ProjetosDetalhadosDTO()
        {
            this.DetalhesPeriodo = new PeriodoDetalhadoDTO();
            this.PeriodoAlocacao = new List<PeriodoAlocadoColaboradorDTO>();
        }
        [JsonIgnore]
        [JsonPropertyName("periodoAlocadoId")]
        public long PeriodoAlocadoId { get; set; }
        [JsonPropertyName("colaboradorAlocadoId")]
        public long ColaboradorAlocadoId { get; set; }
        [JsonPropertyName("cpfColaborador")]
        public string CpfColaborador { get; set; }
        [JsonPropertyName("codigoTBD")]
        public int? CodigoTBD { get; set; }
        [JsonPropertyName("nomeProjeto")]
        public string NomeProjeto { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string LabelProjetoCliente { get; set; }
        [JsonPropertyName("idProjeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("nomeGestor")]
        public string NomeGestor { get; set; }
        [JsonPropertyName("codigoGestor")]
        public string CodigoGestor { get; set; }
        [JsonIgnore]
        [JsonPropertyName("incluiFimDeSemana")]
        public bool IncluiFimDeSemana { get; set; }
        [JsonPropertyName("periodoAlocacao")]
        public List<PeriodoAlocadoColaboradorDTO> PeriodoAlocacao { get; set; }
        [JsonPropertyName("DetalhesPeriodo")]
        public PeriodoDetalhadoDTO DetalhesPeriodo { get; set; }
    }
}