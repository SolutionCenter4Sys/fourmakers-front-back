using System;

namespace DataTransferObject.Domain.SRS
{
    public class RecomendarVagasDTO
    {
        public long SkillVagaId { get; set; }
        public int TipoSkillId { get; set; }
        public int SkillId { get; set; }
        public int SkillNivelId { get; set; }
        public long VagasSrsId { get; set; }
        public string Cargo { get; set; }
        public string Nivel { get; set; }
        public string StatusVaga { get; set; }
        public string Modalidade { get; set; }
        public string Estado { get; set; }
        public DateTime DataAbertura { get; set; }
        public virtual DetalheDTO Vaga { get; set; }
        public double compatibilidadeVaga { get; set; }
    }
}