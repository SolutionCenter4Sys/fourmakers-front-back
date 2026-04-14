using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projeto_org
    {
        public tb_projeto_org()
        {
            tb_colaborador_apontamento = new HashSet<tb_colaborador_apontamento>();
            tb_projeto_org_atividade = new HashSet<tb_projeto_org_atividade>();
        }

        public string cod_projeto { get; set; }
        public string projeto { get; set; }
        public string cod_cliente { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public int cod_status { get; set; }
        public string status { get; set; }
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public string cod_proposta { get; set; }
        public decimal qtd_horas_planejadas { get; set; }
        public decimal qtd_horas_executadas { get; set; }
        public int tb_org_id { get; set; }
        public bool prioritario { get; set; }
        public string codigo_oportunidade { get; set; }
        public int permite_apont_sem_alocacao { get; set; }
        public int permite_apont_sem_alocacao_outro_colab { get; set; }
        public string cod_cliente_registro_carga { get; set; }
        public string nome_cliente_registro_carga { get; set; }
        public string tipo_cadastro { get; set; }

        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual ICollection<tb_projeto_org_atividade> tb_projeto_org_atividade { get; set; }
    }
}