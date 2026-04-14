using System.Collections.Generic;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class InputSubstituirTbdColaborador
    {
        public string CodigoProjeto { get; set; }
        public int CodigoTbdSubstituido { get; set; }
        public string CpfColaboradorSubstituto { get; set; }
        public string CodigoColaboradorSubstituto { get; set; }
        public List<long> PeriodosSubstituidos { get; set; }
        public int OrgId { get; set; }
    }
}