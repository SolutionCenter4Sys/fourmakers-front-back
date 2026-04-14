using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoInteracaoDTO
    {
        public string CodigoInterno { get; set; }
        public DateTime? DataPrimeiraEntrega { get; set; }
        public bool? Visualizado { get; set; }
        public DateTime? DataVisualizado { get; set; }
        public bool? ConfirmouLeitura { get; set; }
        public DateTime? DataConfirmouLeitura { get; set; }
        public string CurtidaEmoji { get; set; }
    }
}
