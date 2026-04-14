using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class ProjetoHorasDTO
    {
        [JsonPropertyName("cd_Projeto")]
        public long CdProjeto { get; set; }

        [JsonPropertyName("nm_Projeto")]
        public string NmProjeto { get; set; }

        [JsonPropertyName("cd_Status_Projeto")]
        public int? CdStatusProjeto { get; set; }
        [JsonPropertyName("nm_Status_Projeto")]
        public string NmStatusProjeto { get; set; }

        [JsonPropertyName("qt_tHoras_Comerciais")]
        public long? QtHorasComerciais { get; set; }

        [JsonPropertyName("qt_Horas_Trabalhadas")]
        public long? QtHorasTrabalhadas { get; set; }
        [JsonPropertyName("codigo_cliente")]
        public long? cdCliente { get; set; }
        [JsonPropertyName("nome_cliente")]
        public string nmCliente { get; set; }
        [JsonPropertyName("data_inicio_projeto")]
        public DateTime? dtInicioProjeto { get; set; }
        [JsonPropertyName("data_fim_projeto")]
        public DateTime? dtFimProj { get; set; }
        [JsonPropertyName("proposta")]
        public string nmPropostas { get; set; }
    }
}