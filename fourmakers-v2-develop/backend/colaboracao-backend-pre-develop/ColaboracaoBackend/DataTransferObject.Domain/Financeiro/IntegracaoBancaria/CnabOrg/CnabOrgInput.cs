using DataTransferObject.Domain.Projeto.GestorExterno;
using DataTransferObject.Domain.Util;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg
{
    public class CnabOrgInput : CnabOrgBase
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public void AtualizarPropriedadesDaClasseBase(CnabOrgBase cnabOrgBase)
        {
            this.AtualizarSafeComPropriedadesDe(cnabOrgBase); //Util padrão do DataTransferObject.Domain.Util
        }

        public void ConfigurarParaPersistencia(int orgId, Guid id)
        {
            TbOrgId = orgId;
            Id = id;
        }

    }
}
