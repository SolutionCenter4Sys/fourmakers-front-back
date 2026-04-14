using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_periodo_alocacao
    {
        public long id { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public double quantidade_horas { get; set; }
        public sbyte inclui_fimdesemana { get; set; }
        public long tb_colaborador_alocado_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string observacao { get; set; }
        public string oportunidade { get; set; }
        public sbyte? prioritario { get; set; }
        public double? percentual { get; set; }

        public virtual tb_colaborador_alocado tb_colaborador_alocado { get; set; }
    }
}