using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain;

namespace DataTransferObject.Domain.Candidato
{
    public class CandidaturaAgrupadaAlteracaoDTO
    {
        public int IdStatus { get; set; }
        public string StatusDescricao { get; set; }
        public string Recrutador { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Comentario { get; set; }
        public string IdComentario { get; set; }
        public string MotivoReprovacao { get; set; }
        public string MotivoDeclinio { get; set; }
        public string RecrutadorResponsavel { get; set; }
        public List<CandidaturaArquivosDTO> Arquivos { get; set; } = new List<CandidaturaArquivosDTO>();
    }

    public class CandidatoVagaLogObjetoDTO
    {
        public string ColaboradorResponsavel { get; set; }
    }
} 