using System;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class HistoricoCandidateEntrevistaDTO
    {
        public int id { get; set; }
        public int candidate_id { get; set; }
        public int joborder_id { get; set; }
        public int analista { get; set; }
        public string prcanalista { get; set; }
        public DateTime dataEntrevista { get; set; }
        public string horaInicioEntrevista { get; set; }
        public string horaFinalEntrevista { get; set; }
        public string notesDPA { get; set; }
    }
}