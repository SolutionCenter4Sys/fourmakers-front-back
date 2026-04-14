using System;

namespace DataTransferObject.Domain.CCH
{
    public class ProjetoHorasFourMakersCCH
    {
        public int cdProjeto { get; set; }
        public string nmProjeto { get; set; }
        public int cdStatusProjeto { get; set; }
        public string dsStatusProjeto { get; set; }
        public int qtHorasComerciais { get; set; }
        public int qtHorasTrabalhadas { get; set; }
        public long? cdCliente { get; set; }
        public string nmCliente { get; set; }
        public DateTime? dtInicioProjeto { get; set; }
        public DateTime? dtFimProj { get; set; }
        public string nmPropostas { get; set; }
    }
}