using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class ProjetosDetalhadosEditarDTO
    {
        public ProjetosDetalhadosEditarDTO()
        {
            this.PeriodoAlocacao = new List<PeriodoAlocadoColaboradorDTO>();
        }
        [JsonIgnore]
        [JsonPropertyName("periodoAlocadoId")]
        public long PeriodoAlocadoId { get; set; }
        [JsonPropertyName("colaboradorAlocadoId")]
        public long ColaboradorAlocadoId { get; set; }

        [JsonPropertyName("codigoTBD")]
        public int? CodigoTBD { get; set; }
        [JsonPropertyName("nomeProjeto")]
        public string NomeProjeto { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }
        public string LabelProjetoCliente { get; set; }
        [JsonPropertyName("idProjeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("nomeGerenteProjeto")]
        public string NomeGerenteProjeto { get; set; }
        [JsonPropertyName("codigoGerenteProjeto")]
        public string CodigoGerenteProjeto { get; set; }
        [JsonIgnore]
        [JsonPropertyName("incluiFimDeSemana")]
        public bool IncluiFimDeSemana { get; set; }
        [JsonPropertyName("periodoAlocacao")]
        public List<PeriodoAlocadoColaboradorDTO> PeriodoAlocacao { get; set; }
    }
}