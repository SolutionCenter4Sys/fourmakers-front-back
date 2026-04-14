using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_colaborador_apontamento
    {
        public Guid tb_colaborador_apontamento_id { get; set; }
        public long horas { get; set; }
        public string justificativa { get; set; }
        public DateTime? data_justificativa { get; set; }
        public string nome_usuario_justificativa { get; set; }
        public DateTime? data_registro { get; set; }
        public int? mes { get; set; }
        public int? ano { get; set; }
        public int? numero_semana { get; set; }
        public int? numero_semana_dia { get; set; }
        public string tipo_apontamento { get; set; }
        public string cod_status_apontamento_grupo { get; set; }
        public string status_apontamento_grupo { get; set; }
        public int? cod_status_apontamento { get; set; }
        public string status_apontamento { get; set; }
        public string cod_projeto { get; set; }
        public string nome_projeto { get; set; }
        public string cod_cliente { get; set; }
        public string nome_cliente { get; set; }
        public bool permite_apont_sem_alocacao { get; set; }
        public bool permite_apont_sem_alocacao_outro_colab { get; set; }
        public string gerente { get; set; }
        public string tipo_gerente { get; set; }
        public string codigo_interno_colaborador_gerente { get; set; }
        public string cod_gerente { get; set; }
        public Guid atividade_id { get; set; }
        public string atividade_descricao { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int? tb_org_id { get; set; }
        public string observacao { get; set; }
    }
}