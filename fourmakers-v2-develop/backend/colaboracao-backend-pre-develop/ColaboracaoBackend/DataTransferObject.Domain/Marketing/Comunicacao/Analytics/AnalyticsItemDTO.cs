using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class AnalyticsItemDTO
    {
        public Guid PublicacaoId { get; set; }
        public string Tipo { get; set; }
        public string Titulo { get; set; }
        public string Onde { get; set; }
        public bool ObrigatorioLeitura { get; set; }
        public DateTime? Data { get; set; }
        public int Likes { get; set; }
        public int Comentarios { get; set; }
        public int Aceites { get; set; }
        public int Visualizacoes { get; set; }
        public int Alcance { get; set; }
        public decimal EngajamentoPercentual { get; set; }
    }
}
