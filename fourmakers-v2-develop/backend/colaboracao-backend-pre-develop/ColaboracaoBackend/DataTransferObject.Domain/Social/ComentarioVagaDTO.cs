using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Candidato;

namespace DataTransferObject.Domain
{
    public class ComentarioVagaDTO
    {
        public string Id { get; set; }
        public string Texto { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string CodigoInternoColaboradorNome { get; set; }
        public Guid VagaId { get; set; }
    }

    public class CriarComentarioVagaDTO
    {
        public string Comentario { get; set; }
        public Guid VagaId { get; set; }
    }

    public class AtualizarComentarioVagaDTO
    {
        public string Comentario { get; set; }
    }
} 