using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_candidato
    {
        public long id { get; set; }
        public string path_curriculo { get; set; }
        public double? pretensao_salarial { get; set; }
        public DateTime data_estagio_processo { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int estagio_processo_seletivo_id { get; set; }
        public string cargo_atual_ultimo { get; set; }
        public decimal? salario_atual_ultimo { get; set; }
        public string tipo_contrato_atual_ultimo { get; set; }
        public bool? aceita_sugestoes_vagas { get; set; }
        public int? modalidade_atual_ultima { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_estagio_processo_seletivo estagio_processo_seletivo { get; set; }
    }
}