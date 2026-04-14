namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria
{
    public class DadosBancariosColaboradorDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string CpfColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public string FormaPagamento { get; set; }
        public string CodigoBanco { get; set; }
        public string Agencia { get; set; }
        public string AgenciaDv { get; set; }
        public string Conta { get; set; }
        public string ContaDv { get; set; }
        public string ChavePix { get; set; }
        public string TipoChavePix { get; set; }
    }
}
