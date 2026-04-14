#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class graficocompetenciacolab
    {
        public long id { get; set; }
        public string descricao { get; set; }
        public long quantidade { get; set; }
        public long? total { get; set; }
    }
}