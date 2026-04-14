using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Candidato;

namespace DataTransferObject.Domain
{
    public class ComentarioCandidaturaDTO
    {
        public string Id { get; set; }
        public string Texto { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string CodigoInternoColaboradorNome { get; set; }
        public string CandidaturaId { get; set; }
        
        public List<CandidaturaArquivosDTO> Arquivos { get; set; }
    }

    public class CriarComentarioCandidaturaDTO
    {
        public string Comentario { get; set; }
        public string CandidaturaId { get; set; }
    }

    public class AtualizarComentarioCandidaturaDTO
    {
        public string Comentario { get; set; }
    }
} 