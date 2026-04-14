namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>
    /// Contagens por status para os "big numbers" da Gestão de Desempenho (PDI).
    /// </summary>
    public class PdiMetricasBigNumbersDTO
    {
        public int NaoIniciado { get; set; }
        public int EmAnalise { get; set; }
        public int EmAndamento { get; set; }
        public int Finalizados { get; set; }
        public int Cancelados { get; set; }
    }
}
