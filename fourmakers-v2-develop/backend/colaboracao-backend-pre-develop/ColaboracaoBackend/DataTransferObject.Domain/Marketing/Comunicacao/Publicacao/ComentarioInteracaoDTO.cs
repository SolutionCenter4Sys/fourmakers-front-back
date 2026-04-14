using System;

namespace DataTransferObject.Domain.Marketing.Comunicacao.Publicacao
{
    public class ComentarioInteracaoDTO
    {
        public string CodigoInterno { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string Emoji { get; set; }
    }
}
