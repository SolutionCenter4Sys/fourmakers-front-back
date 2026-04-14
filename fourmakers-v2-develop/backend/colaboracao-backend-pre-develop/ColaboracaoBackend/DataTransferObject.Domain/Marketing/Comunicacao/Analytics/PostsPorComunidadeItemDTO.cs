using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class PostsPorComunidadeItemDTO
    {
        public Guid ComunidadeId { get; set; }
        public string ComunidadeNome { get; set; }
        public int Membros { get; set; }
        public int Posts { get; set; }
        public int Likes { get; set; }
        public int Comentarios { get; set; }
        public int Aceites { get; set; }
        public int Visualizacoes { get; set; }
        public decimal EngajamentoPercentual { get; set; }
    }
}
