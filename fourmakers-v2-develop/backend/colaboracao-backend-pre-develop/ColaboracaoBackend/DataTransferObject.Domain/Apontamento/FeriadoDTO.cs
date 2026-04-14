using System;

namespace DataTransferObject.Domain.Apontamento
{
    public class FeriadoDTO
    {
        public DateTime Data { get; set; }
        public string Nome { get; set; }
        public TipoFeriado Tipo { get; set; }
    }

    public enum TipoFeriado
    {
        NACIONAL,
        ESTADUAL,
        MUNICIPAL
    }
}