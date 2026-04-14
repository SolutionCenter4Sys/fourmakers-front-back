using System;

namespace DataTransferObject.Domain.Apontamento.FecharAlterarPeriodo
{
    public class FecharAlterarPeriodoDTO
    {
        public DateTime DataFim { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}