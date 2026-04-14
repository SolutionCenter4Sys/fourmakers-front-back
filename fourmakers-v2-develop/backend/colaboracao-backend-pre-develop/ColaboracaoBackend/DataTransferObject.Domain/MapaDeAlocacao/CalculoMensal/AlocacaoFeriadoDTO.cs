using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.CalculoMensal
{
    public class FeriadoAlocacaoDTO
    {
        public DateTime Data { get; set; }
        public string Nome { get; set; }
        public TipoFeriadoAlocacao Tipo { get; set; }
    }

    public enum TipoFeriadoAlocacao
    {
        NACIONAL,
        ESTADUAL,
        MUNICIPAL
    }
}