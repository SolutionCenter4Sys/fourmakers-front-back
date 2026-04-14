using System;

namespace DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoInput
{
    public class ValidaApontamentoInput
    {
        public long Horas { get; set; }
        public string DataRegistro { get; set; }
        public string ColaboradorApontamentoId { get; set; }
        public DateTime? DataColetaDeDados { get; set; }
        public string CpfRequest { get; set; }
        public string CpfColaborador { get; set; }
        public string ProjetoId { get; set; }
        public bool HoraZerada { get; set; } = false;
        public string AtividadeId { get; set; }

    }
}
