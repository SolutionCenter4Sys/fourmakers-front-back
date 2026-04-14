#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_filtro_mapaalocacao
    {
        public string codigo_interno_colaborador { get; set; }
        public string nome_completo { get; set; }
        public sbyte ativo { get; set; }
        public int tb_org_id { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public string hardskills { get; set; }
        public string idiomas { get; set; }
    }
}