using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projeto
    {
        public tb_projeto()
        {
            tb_colaborador_projeto = new HashSet<tb_colaborador_projeto>();
        }

        public long id { get; set; }
        public string nome_projeto { get; set; }
        public DateTime data_inicial { get; set; }
        public DateTime data_final { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string tb_empresa_cnpj { get; set; }

        public virtual tb_empresa tb_empresa_cnpjNavigation { get; set; }
        public virtual ICollection<tb_colaborador_projeto> tb_colaborador_projeto { get; set; }
    }
}