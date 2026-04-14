using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Metadados de paginação para listas de métricas PDI.</summary>
    public class PdiMetricasListaPaginacaoDTO
    {
        /// <summary>Página atual (1-based).</summary>
        public int Pagina { get; set; }

        /// <summary>Itens por página.</summary>
        public int TamanhoPagina { get; set; }

        /// <summary>Total de registros no escopo do filtro (todas as páginas).</summary>
        public int TotalItens { get; set; }

        /// <summary>Total de páginas (0 se TotalItens = 0).</summary>
        public int TotalPaginas { get; set; }
    }

    /// <summary>Lista paginada (combos, grids auxiliares).</summary>
    public class PdiMetricasListagemPaginadaDTO<T>
    {
        public IReadOnlyList<T> Itens { get; set; }
        public PdiMetricasListaPaginacaoDTO Paginacao { get; set; }
    }
}
