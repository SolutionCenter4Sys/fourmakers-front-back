using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador
{
    public class RubricaColaboradorBase
    {
        public decimal Valor { get; set; }
        public decimal Percentual { get; set; }
        public string Hora { get; set; }
        public Guid RubricaId { get; set; }
        public Guid CodigoInternoColaborador { get; set; }
        public string CodigoRubricaFrequencia { get; set; }
        public string Observacao { get; set; }
        public int MesInicial { get; set; }
        public int AnoInicial { get; set; }
        public int? MesFinal { get; set; } = null;
        public int? AnoFinal { get; set; } = null;
        public string CodigoCargaRubrica { get; set; }

    }
}