using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Resultado paginado da listagem de PDIs do time.
    /// </summary>
    public class PdiListagemTimeResult
    {
        public IReadOnlyList<PdiResumoTimeDTO> Items { get; set; }
        public int TotalCount { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas { get; set; }
    }
}
