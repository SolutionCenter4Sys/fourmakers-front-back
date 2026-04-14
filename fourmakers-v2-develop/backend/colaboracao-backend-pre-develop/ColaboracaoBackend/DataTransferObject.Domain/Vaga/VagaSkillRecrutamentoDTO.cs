using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaSkillRecrutamentoDTO
    {
        public string Id { get; set; }

        public long SkillId { get; set; }
        public string SkillDescription { get; set; }
        public long SkillNivelId { get; set; }
        public string SkillNivelDescription { get; set; }
        public int TipoSkillId { get; set; }
        public string TypeSkillsDescription { get; set; }
        public string VagaId { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAlteracao { get; set; }
        public bool Relevante { get; set; }
    }
} 