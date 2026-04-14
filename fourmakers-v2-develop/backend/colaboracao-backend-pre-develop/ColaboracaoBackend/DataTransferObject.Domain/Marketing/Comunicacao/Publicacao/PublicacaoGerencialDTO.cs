namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoGerencialDTO
    {
        public int QuantidadeAgendados { get; set; }
        public int QuantidadePendentesAprovacao { get; set; }
        public string PublicacaoStatus { get; set; }
        public string AprovacaoStatus { get; set; }
    }
}
