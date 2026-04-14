using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{

    public class RetornoAnaliseHoleriteDTO
    {
        /// <summary>
        /// Indica se houve divergência na análise
        /// </summary>
        [JsonPropertyName("houve_divergencia")]
        public bool HouveDivergencia { get; set; }

        /// <summary>
        /// Número total de divergências encontradas
        /// </summary>
        [JsonPropertyName("numero_de_divergencias")]
        public int NumeroDeDivergencias { get; set; }

        /// <summary>
        /// Lista detalhada das divergências encontradas
        /// </summary>
        [JsonPropertyName("relatorio_divergencias")]
        public List<ItemAnaliseDTO> RelatorioDivergencias { get; set; } = new List<ItemAnaliseDTO>();

        /// <summary>
        /// Número total de itens verificados com sucesso
        /// </summary>
        [JsonPropertyName("numero_de_verificados")]
        public int NumeroDeVerificados { get; set; }

        /// <summary>
        /// Lista detalhada dos itens verificados com sucesso
        /// </summary>
        [JsonPropertyName("relatorio_verificados")]
        public List<ItemAnaliseDTO> RelatorioVerificados { get; set; } = new List<ItemAnaliseDTO>();

        /// <summary>
        /// Número total de itens informativos
        /// </summary>
        [JsonPropertyName("numero_de_informativos")]
        public int NumeroDeInformativos { get; set; }

        /// <summary>
        /// Lista detalhada dos itens informativos
        /// </summary>
        [JsonPropertyName("relatorio_informativos")]
        public List<ItemAnaliseDTO> RelatorioInformativos { get; set; } = new List<ItemAnaliseDTO>();
    }

    public class ItemAnaliseDTO
    {
        /// <summary>
        /// Campo ou área que foi analisada
        /// </summary>
        [JsonPropertyName("campo")]
        public string Campo { get; set; }

        /// <summary>
        /// Status do item analisado (DIVERGENTE, VERIFICADO, INFORMATIVO)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Mensagem descritiva sobre o resultado da análise
        /// </summary>
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Regra aplicada na análise
        /// </summary>
        [JsonPropertyName("regra")]
        public string Regra { get; set; }

        /// <summary>
        /// Detalhes do cálculo realizado (null se não aplicável)
        /// </summary>
        [JsonPropertyName("calculo_detalhado")]
        public CalculoDetalhadoDTO CalculoDetalhado { get; set; }

        /// <summary>
        /// Detalhes da divergência (null se não houver divergência)
        /// </summary>
        [JsonPropertyName("divergencia")]
        public DivergenciaDetalheDTO Divergencia { get; set; }
    }

    public class CalculoDetalhadoDTO
    {
        /// <summary>
        /// Fórmula utilizada no cálculo
        /// </summary>
        [JsonPropertyName("formula")]
        public string Formula { get; set; }

        /// <summary>
        /// Lista de passos do cálculo
        /// </summary>
        [JsonPropertyName("passos")]
        public List<string> Passos { get; set; } = new List<string>();

        /// <summary>
        /// Lista de variáveis utilizadas no cálculo (formato "chave: valor")
        /// </summary>
        [JsonPropertyName("variaveis")]
        public List<string> Variaveis { get; set; } = new List<string>();
    }

    public class DivergenciaDetalheDTO
    {
        /// <summary>
        /// Valor calculado/esperado pela FourMakers (pode ser string ou number)
        /// </summary>
        [JsonPropertyName("valor_fourmakers")]
        public string ValorFourmakers { get; set; }

        /// <summary>
        /// Valor encontrado na contabilidade/holerite
        /// </summary>
        [JsonPropertyName("valor_contabilidade")]
        public string ValorContabilidade { get; set; }
    }

    public class MetadadosConsultaDTO
    {
        /// <summary>
        /// Tempo de processamento em segundos
        /// </summary>
        [JsonPropertyName("tempo_processamento_segundos")]
        public double TempoProcessamentoSegundos { get; set; }

        /// <summary>
        /// Número de tokens de entrada utilizados (pode ser null se API não retornar)
        /// </summary>
        [JsonPropertyName("numero_tokens_entrada")]
        public int? NumeroTokensEntrada { get; set; }

        /// <summary>
        /// Número de tokens de saída gerados (pode ser null se API não retornar)
        /// </summary>
        [JsonPropertyName("numero_tokens_saida")]
        public int? NumeroTokensSaida { get; set; }

        /// <summary>
        /// Número total de tokens utilizados (pode ser null se API não retornar)
        /// </summary>
        [JsonPropertyName("numero_tokens_total")]
        public int? NumeroTokensTotal { get; set; }

        /// <summary>
        /// Modelo de IA utilizado na análise
        /// </summary>
        [JsonPropertyName("modelo_utilizado")]
        public string ModeloUtilizado { get; set; }

        /// <summary>
        /// Provedor de IA utilizado (openai, gemini)
        /// </summary>
        [JsonPropertyName("provedor_utilizado")]
        public string ProvedorUtilizado { get; set; }

        /// <summary>
        /// Timestamp da consulta (formato ISO datetime)
        /// </summary>
        [JsonPropertyName("timestamp_consulta")]
        public string TimestampConsulta { get; set; }

        /// <summary>
        /// Custo estimado em USD (pode ser null)
        /// </summary>
        [JsonPropertyName("custo_estimado_usd")]
        public double? CustoEstimadoUsd { get; set; }
    }
}