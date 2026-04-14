using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Resultado completo de métricas PDI: big numbers, ativos (em análise + em andamento) e históricos (finalizados + cancelados).
    /// </summary>
    public class PdiMetricasResultDTO
    {
        public PdiMetricasBigNumbersDTO BigNumbers { get; set; }

        /// <summary>Paginação da lista <see cref="Itens"/> (busca principal). Null nas visões colaborador/gestor.</summary>
        public PdiMetricasListaPaginacaoDTO ListaPaginacao { get; set; }

        /// <summary>Lista da página atual (filtrada por status quando informado). Ver <see cref="ListaPaginacao"/>.</summary>
        public IReadOnlyList<PdiMetricaItemDTO> Itens { get; set; }
        public IReadOnlyList<PdiMetricaItemDTO> Ativos { get; set; }
        public IReadOnlyList<PdiMetricaItemDTO> Historicos { get; set; }
    }
}
