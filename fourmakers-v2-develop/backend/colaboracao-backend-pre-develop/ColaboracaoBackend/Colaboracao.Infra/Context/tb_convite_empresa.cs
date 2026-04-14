using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_convite_empresa
    {
        public long id { get; set; }
        public string nome_completo { get; set; }
        public string cpf { get; set; }
        public string email { get; set; }
        public string token { get; set; }
        public DateTime validade { get; set; }
        public string tb_empresa_cnpj { get; set; }

        public virtual tb_empresa tb_empresa_cnpjNavigation { get; set; }
    }
}