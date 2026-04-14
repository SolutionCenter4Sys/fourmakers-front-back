using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class AtualizarComentarioRequestDTO
    {
        public Guid ComentarioId { get; set; }
        public string Conteudo { get; set; }
    }
}
