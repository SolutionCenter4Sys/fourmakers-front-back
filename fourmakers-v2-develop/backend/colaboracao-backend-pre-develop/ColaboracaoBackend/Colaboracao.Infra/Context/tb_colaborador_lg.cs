using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_lg
    {
        public tb_colaborador_lg()
        {
            tb_holerite = new HashSet<tb_holerite>();
        }

        public long id { get; set; }
        public string cpf { get; set; }
        public long org_id { get; set; }
        public long lg_pessoa_id { get; set; }
        public long tb_holerite_id { get; set; }
        public string lg_matricula { get; set; }
        public int codigo_empresa { get; set; }

        public virtual ICollection<tb_holerite> tb_holerite { get; set; }
    }
}