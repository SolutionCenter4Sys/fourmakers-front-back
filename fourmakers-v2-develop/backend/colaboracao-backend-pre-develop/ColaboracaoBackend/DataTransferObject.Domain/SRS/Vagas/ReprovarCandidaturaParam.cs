
using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class ReprovarCandidaturaParam
    {
        public string IdCandidatura { get; set; }
        public string IdMotivoReprovacao { get; set; }
        public string Comentario { get; set; }
    }
}