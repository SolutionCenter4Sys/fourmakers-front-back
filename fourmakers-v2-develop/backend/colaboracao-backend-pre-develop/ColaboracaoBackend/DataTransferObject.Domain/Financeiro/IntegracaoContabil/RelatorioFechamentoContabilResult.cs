namespace DataTransferObject.Domain.Financeiro.IntegracaoContabil
{
    public class RelatorioFechamentoContabilResult
    {
        public string Origem { get; set; }
        public string Competencia { get; set; }
        public string NomeColaborador { get; set; }
        public string Tipo { get; set; }
        public string Valor { get; set; }
        public string Categoria { get; set; }
    }
}