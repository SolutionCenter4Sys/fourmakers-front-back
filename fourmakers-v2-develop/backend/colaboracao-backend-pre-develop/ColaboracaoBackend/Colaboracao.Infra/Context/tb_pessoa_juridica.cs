#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_pessoa_juridica
    {
        public string cnpj { get; set; }
        public string razao_social { get; set; }
        public string nome_fantasia { get; set; }
        public string agencia { get; set; }
        public string conta_digito { get; set; }
        public int tb_regime_tributario_id { get; set; }
        public string tb_bancos_codigo { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_bancos tb_bancos_codigoNavigation { get; set; }
        public virtual tb_regime_tributario tb_regime_tributario { get; set; }
    }
}