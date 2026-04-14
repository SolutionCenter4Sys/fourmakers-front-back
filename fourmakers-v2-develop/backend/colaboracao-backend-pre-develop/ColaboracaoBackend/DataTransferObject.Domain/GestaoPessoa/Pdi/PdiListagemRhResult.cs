using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiListagemRhResult
    {
        public IReadOnlyList<PdiResumoRhDTO> Items { get; set; }
        public int TotalCount { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas { get; set; }
    }
}
