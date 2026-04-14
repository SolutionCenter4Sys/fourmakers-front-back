using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class ConciliacaoFolhaPontoDivergenciaDTO
    {
        /// <summary>
        /// ID único da divergência
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Campo onde foi encontrada a divergência (pode ser nulo)
        /// </summary>
        public string CampoDivergencia { get; set; }

        /// <summary>
        /// Mensagem descritiva da divergência
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Valor esperado (pode ser nulo)
        /// </summary>
        public string ValorEsperado { get; set; }

        /// <summary>
        /// Valor encontrado na contabilidade (pode ser nulo)
        /// </summary>
        public string ValorContabilidade { get; set; }

        /// <summary>
        /// Status do item analisado (DIVERGENTE, VERIFICADO, INFORMATIVO)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Regra aplicada na análise
        /// </summary>
        public string Regra { get; set; }

        /// <summary>
        /// Fórmula utilizada no cálculo
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Lista de passos do cálculo
        /// </summary>
        public List<string> Passos { get; set; } = new List<string>();

        /// <summary>
        /// Lista de variáveis utilizadas no cálculo (formato "chave: valor")
        /// </summary>
        public List<string> Variaveis { get; set; } = new List<string>();

        /// <summary>
        /// Indica se este item representa uma divergência
        /// </summary>
        public bool EhDivergencia { get; set; }

        /// <summary>
        /// ID da conciliação de folha ponto do colaborador relacionada
        /// </summary>
        [JsonIgnore]
        public string TbConciliacaoFolhaPontoColaboradorId { get; set; }
    }
} 