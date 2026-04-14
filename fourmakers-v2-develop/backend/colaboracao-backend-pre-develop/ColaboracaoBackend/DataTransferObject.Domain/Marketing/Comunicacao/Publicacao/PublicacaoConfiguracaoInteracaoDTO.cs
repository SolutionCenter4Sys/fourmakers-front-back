namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class PublicacaoConfiguracaoInteracaoDTO
    {
        public bool RequerConfirmacaoLeitura { get; set; }
        public bool PermiteComentarios { get; set; }
        public bool PermiteCurtidas { get; set; }
        public bool Fixada { get; set; }
        public bool PermiteDownload { get; set; }
        /// <summary>Quando true, a publicação não aparece no feed.</summary>
        public bool OcultarNoFeed { get; set; }
    }
}
