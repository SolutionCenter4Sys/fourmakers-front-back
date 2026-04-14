using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_usuario_permissao_log
    {
        public int id { get; set; }
        public int tb_org_id { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string operacao { get; set; }
        public int? tb_usuario_id { get; set; }
        public int? tb_grupo_acesso_id { get; set; }
        public int? tb_funcionalidade_sistema_id { get; set; }
    }
}