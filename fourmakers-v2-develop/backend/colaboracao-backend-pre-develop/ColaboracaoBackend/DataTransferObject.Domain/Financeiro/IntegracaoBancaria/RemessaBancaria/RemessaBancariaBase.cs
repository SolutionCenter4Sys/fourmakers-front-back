using System;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg
{
    public class RemessaBancariaBase
    {
        public int TbOrgId { get; set; }
        public string Diretoria { get; set; }
        public string CodigoBanco { get; set; }
        public string Agencia { get; set; }
        public string AgenciaDv { get; set; }
        public string Conta { get; set; }
        public string ContaDV { get; set; }
        public string CodigoConvenio { get; set; }
        public string CnpjEmpresa { get; set; }
        public string FormaPagamento { get; set; }
    }
}