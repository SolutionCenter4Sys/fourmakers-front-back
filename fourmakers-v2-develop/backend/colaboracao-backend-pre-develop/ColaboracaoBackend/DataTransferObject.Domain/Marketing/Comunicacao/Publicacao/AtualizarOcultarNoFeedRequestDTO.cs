using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    /// <summary>
    /// Request para alterar apenas o bit ocultar_no_feed da publicação.
    /// </summary>
    public class AtualizarOcultarNoFeedRequestDTO
    {
        public Guid PublicacaoId { get; set; }
        public bool OcultarNoFeed { get; set; }
    }
}
