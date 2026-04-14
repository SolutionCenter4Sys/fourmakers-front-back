namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaConsolidadaDTO
    {
        public string idCompetencia { get; set; }
        public string descricaoCompetencia { get; set; }
        public int qtdUsuariosCompetencia { get; set; }
        public bool ativo { get; set; }
    }
}