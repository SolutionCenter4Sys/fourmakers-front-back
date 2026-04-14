#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class buscacolaborador
    {
        public string documento_colaborador { get; set; }
        public sbyte? ativo { get; set; }
        public sbyte? candidato { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string BuscaGR1 { get; set; }
        public string BuscaGR2 { get; set; }
        public string BuscaGR3 { get; set; }
        public string BuscaGR4 { get; set; }
    }
}