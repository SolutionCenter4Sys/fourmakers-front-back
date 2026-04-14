using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class AtualizarPublicacaoRequestDTO : InserirPublicacaoRequestDTO
    {
        public Guid PublicacaoId { get; set; }
    }
}
