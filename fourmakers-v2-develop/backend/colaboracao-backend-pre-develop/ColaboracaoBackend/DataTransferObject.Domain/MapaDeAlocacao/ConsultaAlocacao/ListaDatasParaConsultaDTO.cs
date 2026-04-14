using System;

namespace DataTransferObject.Domain.MapaDeAlocacao.ConsultaAlocacao
{
    public class ListaConsultaDatasDTO
    {
        public DateTime DataInicioPeriodo { get; set; }
        public DateTime DataFimPeriodo { get; set; }
        public long PeriodoId { get; set; }
        public bool IncluiFinalDeSemana { get; set; }
        public double QuantidadeHoras { get; set; }
    }
}