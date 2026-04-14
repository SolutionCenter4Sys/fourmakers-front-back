using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class AnalyticsResumoRequestDTO
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        /// <summary>Filtro opcional: "post" ou "comunicado".</summary>
        public string TipoConteudo { get; set; }
        public string Texto { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanhoPagina { get; set; } = 20;
    }
}
