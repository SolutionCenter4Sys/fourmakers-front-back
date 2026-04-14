using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class ListarProjetosColaboradorInput
    {
        public string CodigoProfissional { get; set; }
        public bool? EhTbd { get; set; }
        public string CodigoGerenteProjeto { get; set; }
        public List<string> ListaCodigoCliente { get; set; }
        public string Status { get; set; }
        public FiltroProjetosPrioritariosEnum? PrioritarioFiltro { get; set; }
    }
}