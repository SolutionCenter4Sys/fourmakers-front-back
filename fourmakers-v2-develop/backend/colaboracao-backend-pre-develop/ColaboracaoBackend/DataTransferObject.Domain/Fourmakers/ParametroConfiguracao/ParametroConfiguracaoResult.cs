using System;

namespace DataTransferObject.Domain.Fourmakers.ParametroConfiguracao
{
    public class ParametroConfiguracaoResult : ParametroConfiguracaoBase
    {
        public Guid Id { get; set; }
        public int OrgId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public int Prioridade { get; set; }
    }
}