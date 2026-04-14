namespace DataTransferObject.Domain.Pricing.Equipe
{
    public class RatecardClienteResult
    {
        public string CargoNome { get; set; }
        public string CargoSenioridadeDescricao { get; set; }
        public decimal CustoHora { get; set; }
        public decimal? PrecoHora { get; set; }
    }
}