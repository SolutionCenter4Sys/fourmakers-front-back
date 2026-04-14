using System;

namespace DataTransferObject.Domain.Projeto.ClienteOrgCrud
{
    public class ClienteOrgCrudResult : ClienteOrgCrudBase
    {
        public Guid Id { get; set; }
        public int OrgId { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}