using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class InserirComentarioRequestDTO
    {
        public Guid PublicacaoId { get; set; }
        public string Conteudo { get; set; }
        /// <summary>Opcional. Preenchido quando for resposta a outro comentário.</summary>
        public Guid? ComentarioPaiId { get; set; }
    }
}
