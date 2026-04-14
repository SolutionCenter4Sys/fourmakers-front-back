using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ForcaMensalColaboradorProjetoDTO
    {
        public double HorasAlocadasNoMes { get; set; }
        public List<MesesColaboradorDTO> MesAno { get; set; }
    }
}