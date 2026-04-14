using System;

namespace DataTransferObject.Domain.Comentario
{
    public class BuscaComentarioDTO
    {
        public long Id { get; set; }
        public int ComentarioTipoId { get; set; }
        public DateTime DataComentario { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public char Email { get; set; }
        public string Texto { get; set; }
        public string TipoComentario { get; set; }
    }
}