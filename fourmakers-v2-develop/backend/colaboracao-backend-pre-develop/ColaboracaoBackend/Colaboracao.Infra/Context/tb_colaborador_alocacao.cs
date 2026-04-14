using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_alocacao
    {
        public int id { get; set; }
        public long tb_projeto_id { get; set; }
        public string tb_colaborador_cpf { get; set; }
        public decimal? disponibilidade { get; set; }
        public sbyte? disponivel_he { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador tb_colaborador_cpfNavigation { get; set; }
    }
}