using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_dependente
    {
        public long id { get; set; }
        public string nome_completo { get; set; }
        public DateTime data_nascimento { get; set; }
        public string rg { get; set; }
        public string cpf { get; set; }
        public sbyte portador_deficiencia { get; set; }
        public string requer_ajuda_qual { get; set; }
        public int tipo_dependente_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_tipo_dependente tipo_dependente { get; set; }
    }
}