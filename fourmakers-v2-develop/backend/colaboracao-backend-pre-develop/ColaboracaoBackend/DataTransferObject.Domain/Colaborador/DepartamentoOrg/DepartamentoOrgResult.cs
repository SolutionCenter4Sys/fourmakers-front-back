using System;

namespace DataTransferObject.Domain.Colaborador.DepartamentoOrg
{
    public class DepartamentoOrgResult : DepartamentoOrgBase
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}