using System;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class HorasColaboradorDTO
    {
        public long Horas { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }
    }
}