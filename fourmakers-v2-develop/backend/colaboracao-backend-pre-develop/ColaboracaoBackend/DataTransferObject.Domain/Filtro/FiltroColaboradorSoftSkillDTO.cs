namespace DataTransferObject.Domain.Filtro
{
    public class FiltroColaboradorSoftSkillDTO
    {
        public string ColaboradorCpf { get; set; }
        public string ColaboradorNome { get; set; }
        public long SoftSkillId { get; set; }
        public long NivelId { get; set; }
    }
}