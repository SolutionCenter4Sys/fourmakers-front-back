using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_perfil
    {
        public tb_perfil()
        {
            tb_perfil_alocacao = new HashSet<tb_perfil_alocacao>();
        }

        public string id { get; set; }
        public int tb_org_id { get; set; }
        public string nome_perfil { get; set; }
        public string codigo_projeto { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string codigo_interno_colaborador_alteracao { get; set; }

        public virtual ICollection<tb_perfil_alocacao> tb_perfil_alocacao { get; set; }
    }
}