using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ListarAlocacoesColabETbdInput
    {
        public string Pesquisa { get; set; }
        public string CodigoUnidade { get; set; }
        public string CodigoDepartamento { get; set; }
        public string CodigoGestorAdm { get; set; }
        public List<string> ListaCodigoColabOuTbd { get; set; }
        public TipoProfissionalEnum FiltroTipoProfissional { get; set; }
        public string CodigoGestorProjeto { get; set; }
        public List<string> ListaCodigoClientes { get; set; }
        public bool ApenasProjetosPrioritarios { get; set; }
        public List<string> ListaCodigoProjetos { get; set; }
        public int CodigoStatusProjeto { get; set; }
        public List<string> Habilidades { get; set; }
        public List<string> Perfis { get; set; }
    }
}