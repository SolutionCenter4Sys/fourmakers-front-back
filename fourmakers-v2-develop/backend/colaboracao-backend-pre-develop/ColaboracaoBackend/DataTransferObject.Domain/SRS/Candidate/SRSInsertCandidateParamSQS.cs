using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class SRSInsertCandidateParamSQS
    {
        public int IdLog { get; set; }
        public string CandCpf { get; set; }

        public int CandidateId { get; set; }

        public string FirstName { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Email1 { get; set; }

        public string EmailFoursys { get; set; }

        public string TipoCargo { get; set; }

        public int IsActive { get; set; }

        public string PendingToSend { get; set; }

        public string Observacao { get; set; }

        public int Operation { get; set; }

        public DateTime? DataOrigem { get; set; }

        public List<SRSInsertCandidateSkillSQS> Hardskill { get; set; }

        public List<SRSInsertCandidateSkillSQS> Softskill { get; set; }

        public List<SRSInsertCandidateSkillSQS> Methodologia { get; set; }

        public List<SRSInsertCandidateSkillSQS> Dominio { get; set; }

        public List<SRSInsertCandidateSkillSQS> Idioma { get; set; }
    }
}