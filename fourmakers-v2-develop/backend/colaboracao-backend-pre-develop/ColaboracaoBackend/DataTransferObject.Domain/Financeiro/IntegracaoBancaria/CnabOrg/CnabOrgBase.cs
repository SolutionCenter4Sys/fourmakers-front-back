using System;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg
{
    public class CnabOrgBase
    {
        public int TbOrgId { get; set; }
        public string CodDiretoria { get; set; }
        public string CodigoBanco { get; set; }
        public string Agencia { get; set; }
        public string AgenciaDv { get; set; }
        public string Conta { get; set; }
        public string ContaDV { get; set; }
        public string CodigoConvenio { get; set; }
        public string CnpjEmpresa { get; set; }
        public string FormaPagamento { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeBanco { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string NumeroLocal { get; set; }
        public string ComplementoEndereco { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Estado { get; set; }
    }
}