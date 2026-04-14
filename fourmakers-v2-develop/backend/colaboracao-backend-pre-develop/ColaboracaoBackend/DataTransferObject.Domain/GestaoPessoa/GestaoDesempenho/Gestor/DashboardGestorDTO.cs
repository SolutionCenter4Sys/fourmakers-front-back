using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor
{
    public class DashboardGestorDTO
    {
        public PainelControleDTO PainelControle { get; set; }
        public List<OneOnOneRegistroCriticoDTO> OneOnOnesComRegistroCritico { get; set; }
    }

    public class PainelControleDTO
    {
        public int QtdTotalColaboradores { get; set; }
        public int QtdOneOnOneEmDia { get; set; }
        public int QtdOneOnOneAtrasado { get; set; }
        public int QtdFeedbackEmDia { get; set; }
        public int QtdFeedbackAtrasado { get; set; }
    }

    public class OneOnOneRegistroCriticoDTO
    {
        public string NomeCompleto { get; set; }
        public DateTime? DataReuniao { get; set; }
        public string DescricaoAnotacoes { get; set; }
    }
}
