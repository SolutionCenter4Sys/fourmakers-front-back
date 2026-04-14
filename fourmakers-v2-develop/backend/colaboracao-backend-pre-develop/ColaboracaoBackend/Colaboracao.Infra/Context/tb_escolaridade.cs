using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_escolaridade
    {
        public long id { get; set; }
        public string instituicao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime? data_termino { get; set; }
        public string descricao { get; set; }
        public string path_diploma { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public int? tipo_diploma_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long? tb_formacao_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_formacao tb_formacao { get; set; }
        public virtual tb_tipo_diploma tipo_diploma { get; set; }
    }
}