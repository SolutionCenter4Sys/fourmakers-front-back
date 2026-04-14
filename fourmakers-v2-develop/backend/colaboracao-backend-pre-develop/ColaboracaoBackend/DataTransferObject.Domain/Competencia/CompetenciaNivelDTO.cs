using DataTransferObject.Domain.Nivel;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaNivelDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public NivelDTO Nivel { get; set; }
    }
}