namespace DataTransferObject.Domain.Filtro
{
    public class FiltroColaboradorCompetenciaDTO
    {
        public string ColaboradorCpf { get; set; }
        public string ColaboradorNome { get; set; }
        public long CompetenciaId { get; set; }
        public long NivelId { get; set; }
    }
}