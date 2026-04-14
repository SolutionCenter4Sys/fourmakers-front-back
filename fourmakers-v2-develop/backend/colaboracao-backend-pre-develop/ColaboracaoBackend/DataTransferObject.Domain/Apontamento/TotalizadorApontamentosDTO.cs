namespace DataTransferObject.Domain.Apontamento
{
    public class TotalizadorApontamentosBigNumbersDTO
    {
        public int QuantidadeTotalColaboradores { get; set; }
        public decimal SomaHorasLancadas { get; set; }
        public string QuantidadeTotalNaoApontado { get; set; }
        public decimal SomaHorasAprovadas { get; set; }
        public decimal SomaHorasPendentes { get; set; }
        public decimal SomaHorasReprovdas { get; set; }
    }
}