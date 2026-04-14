namespace DataTransferObject.Domain.Competencia
{
    public class AdicionarCompetenciaDTO
    {
        public long IdCompetencia { get; set; }
        public string DescricaoCompetencia { get; set; }
        public bool Ativo { get; set; }
        public bool Pendente { get; set; }
    }
}