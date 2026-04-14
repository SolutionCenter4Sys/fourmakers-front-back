using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class EstatisticasCursoDTO
    {
        public string Descricao { get; set; }
        public List<EstatisticasSemestreDTO> Semestres { get; set; }
        public int Total { get; set; }
    }

    public class EstatisticasSemestreDTO
    {
        public int Semestre { get; set; }
        public int Colaboradores { get; set; }
    }
}