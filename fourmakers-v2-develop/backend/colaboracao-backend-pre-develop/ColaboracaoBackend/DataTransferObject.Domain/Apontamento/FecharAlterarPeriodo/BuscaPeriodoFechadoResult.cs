using System;

namespace DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo
{
    public class BuscaPeriodoFechadoResult
    {
        public int? Id { get; set; }
        public DateTime? DataFim { get; set; }
    }
}