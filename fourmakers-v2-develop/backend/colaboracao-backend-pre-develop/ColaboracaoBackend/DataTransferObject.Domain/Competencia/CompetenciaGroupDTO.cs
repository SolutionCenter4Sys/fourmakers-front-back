using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaGroupDTO
    {
        public long Id { get; set; }
        public string Descricao { get; set; }
        public int? TipoIdSRS { get; set; }
        public int? Confirmada { get; set; }
    }
}
