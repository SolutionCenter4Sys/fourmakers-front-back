#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_gestores_org
    {
        public string cod_colaborador_superior { get; set; }
        public string nome_completo { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public int tb_org_id { get; set; }
        public sbyte ativo { get; set; }
        public string cod_departamento { get; set; }
    }
}