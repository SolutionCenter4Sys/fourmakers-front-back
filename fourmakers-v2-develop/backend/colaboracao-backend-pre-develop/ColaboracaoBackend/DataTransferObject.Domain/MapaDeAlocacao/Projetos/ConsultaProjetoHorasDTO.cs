using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.Projetos
{
    public class ConsultaProjetoHorasDTO
    {
        [JsonPropertyName("codigo_projeto")]
        public string CodigoProjeto { get; set; }
        [JsonPropertyName("nome_projeto")]
        public string NomeProjeto { get; set; }
        [JsonPropertyName("proposta")]
        public string Proposta { get; set; }
        [JsonPropertyName("cliente")]
        public string Cliente { get; set; }
        [JsonPropertyName("codigo_cliente")]
        public string CodigoCliente { get; set; }
        [JsonPropertyName("horas_tecnicas")]
        public long HorasTecnicas { get; set; } //(horasComerciais)
        [JsonPropertyName("horas_alocadas")]
        public double HorasAlocadas { get; set; } //(calcular a quantidade total de horas alocadas por colaborador no projeto e somar o resultado)
        [JsonPropertyName("horas_disponiveis")]
        public double HorasDisponiveis { get; set; } //(Fórmula: (HorasTecnicas - HorasAlocadas)/HorasTecnicas)
        [JsonPropertyName("quantidade_alocados")]
        public int QuantidadeAlocados { get; set; } //(Contar a quantidade de coaboradores alocados no projeto)
        [JsonPropertyName("status_projeto")]
        public string StatusProjeto { get; set; } //(nmStatusProjeto)
        [JsonPropertyName("data_inicio")]
        public DateTime InicioProjeto { get; set; }
        [JsonPropertyName("data_fim")]
        public DateTime FimProjeto { get; set; }
        [JsonPropertyName("nome_gerente_projeto")]
        public string NomeGerenteProjeto { get; set; }
        [JsonPropertyName("cpf_gerente_projeto")]
        public string CpfGerenteProjeto { get; set; }
        [JsonPropertyName("codigo_gerente_projeto")]
        public string CodigoGerenteProjeto { get; set; }
        public int QuantidadeDeGestores { get; set; }
        public int QuantidadeDeTBDS { get; set; }
    }
}