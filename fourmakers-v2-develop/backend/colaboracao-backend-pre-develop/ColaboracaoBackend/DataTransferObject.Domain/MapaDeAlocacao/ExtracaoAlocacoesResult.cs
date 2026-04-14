using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ExtracaoAlocacoesResult
    {
        [JsonPropertyName("codigoColaborador")]
        public string CodigoColaborador { get; set; }
        [JsonPropertyName("recurso")]
        public string Recurso { get; set; }

        [JsonPropertyName("tbd")]
        public string Tbd { get; set; }

        [JsonPropertyName("codigoCliente")]
        public string CodigoCliente { get; set; }
        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }
        [JsonPropertyName("codigoProjeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("projeto")]
        public string Projeto { get; set; }

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataTermino")]
        public DateTime DataTermino { get; set; }

        [JsonPropertyName("notas")]
        public string Observacao { get; set; }

        [JsonPropertyName("percentual")]
        public decimal? Percentual { get; set; }

        [JsonPropertyName("IncluiFimDeSemana")]
        public string IncluiFimDeSemana { get; set; }

        [JsonPropertyName("Oportunidade")]
        public string Oportunidade { get; set; }

        [JsonPropertyName("Prioritario")]
        public string Prioritario { get; set; }
        [JsonPropertyName("StatusColaborador")]
        public string StatusColaborador { get; set; }
    }
}