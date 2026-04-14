using System;

namespace DataTransferObject.Domain.Fourmakers.Parametro
{
    public class ParametroResult : ParametroBase
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}