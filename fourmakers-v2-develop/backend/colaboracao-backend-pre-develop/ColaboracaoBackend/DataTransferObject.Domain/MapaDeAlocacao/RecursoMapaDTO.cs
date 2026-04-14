using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class RecursoMapaDTO
    {
        public string Nome { get; set; }
        public bool EhTbd { get; set; }
        public string CpfColaborador { get; set; }
        public string? CodigoColaborador { get; set; }
        public int? CodigoTbd { get; set; }
        public string HardSkills { get; set; }

        public string Idioma { get; set; }

        public string Diretoria { get; set; }
        public string CodigoDiretoria { get; set; }
        public List<AlocacaoDTO> Alocacao { get; set; }
    }
}