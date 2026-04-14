using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_endereco
    {
        public tb_endereco()
        {
            tb_colaborador = new HashSet<tb_colaborador>();
        }

        public long id { get; set; }
        public string cep { get; set; }
        public string endereco { get; set; }
        public int? numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string com_quem_mora { get; set; }
        public string internacional_linha_um { get; set; }
        public string internacional_linha_dois { get; set; }

        public virtual ICollection<tb_colaborador> tb_colaborador { get; set; }
    }
}