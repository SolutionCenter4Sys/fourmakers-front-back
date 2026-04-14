using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projetohora_cch
    {
        public long cdProjeto { get; set; }
        public string nmProjeto { get; set; }
        public int? cdStatusProjeto { get; set; }
        public string nmStatusProjeto { get; set; }
        public long? horasTecnicas { get; set; }
        public long? qtHorasTrabalhadas { get; set; }
        public long? cdCliente { get; set; }
        public string nmCliente { get; set; }
        public DateTime? dtInicioProjeto { get; set; }
        public DateTime? dtFimProj { get; set; }
        public string nmPropostas { get; set; }
    }
}