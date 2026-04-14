using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto.ClienteOrgCrud
{
    public class ClienteOrgCrudInput : ClienteOrgCrudBase
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public int OrgId { get; set; }
        [JsonIgnore]
        public string CodigoInternoColaboradorAlteracao { get; set; }

        public void ConfigurarParaPersistencia(int orgId, string codigoInternoColaboradorAlteracao, Guid id)
        {
            OrgId = orgId;
            CodigoInternoColaboradorAlteracao = codigoInternoColaboradorAlteracao;

            Id = id;

            if (NomeCliente != null)
            {
                NomeCliente = NomeCliente.ToUpperInvariant();
            }
        }
    }
}