using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_empresa
    {
        public tb_empresa()
        {
            tb_convite_empresa = new HashSet<tb_convite_empresa>();
            tb_empresa_usuario = new HashSet<tb_empresa_usuario>();
            tb_projeto = new HashSet<tb_projeto>();
        }

        public string cnpj { get; set; }
        public string nome_fantasia { get; set; }
        public string razao_social { get; set; }
        public string site { get; set; }
        public string linkedin { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_convite_empresa> tb_convite_empresa { get; set; }
        public virtual ICollection<tb_empresa_usuario> tb_empresa_usuario { get; set; }
        public virtual ICollection<tb_projeto> tb_projeto { get; set; }
    }
}