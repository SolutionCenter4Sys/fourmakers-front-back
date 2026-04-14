namespace DataTransferObject.Domain.Apontamento
{
    public class TotalizadorApontamentosDTO
    {
        public int QuantidadeTotalColaboradores { get; set; }
        public int QuantidadeTotalProjetos { get; set; }
        public decimal SomaHoras { get; set; }
    }
}