using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_historico_competencia
    {
        public Guid id { get; set; }
        public string tipo_competencia_enum { get; set; }
        public string descricao_competencia { get; set; }
        public string situacao { get; set; }
        public string observacao { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string codigo_interno_colaborador_alteracao { get; set; }
    }
}