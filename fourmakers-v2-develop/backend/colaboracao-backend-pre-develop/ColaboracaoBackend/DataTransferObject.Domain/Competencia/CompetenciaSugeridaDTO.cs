using System;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaSugeridaDTO
    {
        public string idCompetencia { get; set; }
        public string descricaoCompetencia { get; set; }
        public int qtdUsuariosCompetencia { get; set; }
        public DateTime dataSolicitacao { get; set; }
    }
}