using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador.Cidadania
{
    public class EstatisticaCidadaniaDTO
    {
        public string Descricao { get; set; }
        public List<EstatisticaCidadaniaStatusDTO> Status { get; set; }
        public int Total { get; set; }
    }

    public class EstatisticaCidadaniaStatusDTO
    {
        public string Descricao { get; set; }
        public int Colaboradores { get; set; }
    }
}