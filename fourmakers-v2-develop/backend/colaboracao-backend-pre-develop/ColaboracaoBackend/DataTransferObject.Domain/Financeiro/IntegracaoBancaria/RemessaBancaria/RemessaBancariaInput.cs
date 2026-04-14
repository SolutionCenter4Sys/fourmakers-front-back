using DataTransferObject.Domain.Util;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg
{
    public class RemessaBancariaInput : RemessaBancariaBase
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public void AtualizarPropriedadesDaClasseBase(CnabOrgBase cnabOrgBase)
        {
            //Util padrão do DataTransferObject.Domain.Util
            //this.AtualizarSafeComPropriedadesDe(cnabOrgBase); 
        }

        public void ConfigurarParaPersistencia(int orgId, Guid id)
        {
            TbOrgId = orgId;
            Id = id;
        }

    }
}
