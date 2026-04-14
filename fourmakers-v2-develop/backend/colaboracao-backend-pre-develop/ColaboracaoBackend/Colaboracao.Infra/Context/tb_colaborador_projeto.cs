using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_projeto
    {
        public long id { get; set; }
        public decimal valor_definido { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_final { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long modalidade_id { get; set; }
        public long projeto_id { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_modalidade modalidade { get; set; }
        public virtual tb_projeto projeto { get; set; }
    }
}