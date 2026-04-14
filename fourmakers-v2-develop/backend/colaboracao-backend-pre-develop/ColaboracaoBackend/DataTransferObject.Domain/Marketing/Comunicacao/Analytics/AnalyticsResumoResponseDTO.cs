using System.Collections.Generic;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Analytics
{
    public class AnalyticsResumoResponseDTO
    {
        public AnalyticsBigNumbersDTO BigNumbers { get; set; }
        public IReadOnlyList<AnalyticsItemDTO> Itens { get; set; }
        public int TotalItens { get; set; }
    }
}
