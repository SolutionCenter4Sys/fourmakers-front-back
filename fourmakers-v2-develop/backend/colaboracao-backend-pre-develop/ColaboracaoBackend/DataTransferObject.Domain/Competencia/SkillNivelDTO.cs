using DataTransferObject.Domain.Nivel;

namespace DataTransferObject.Domain.Competencia
{
    public class SkillNivelDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public NivelDTO? Nivel { get; set; }
        public string TipoSkill { get; set; }
        public bool Relevante { get; set; }
        public string PerfilId  { get; set; }
    }
}