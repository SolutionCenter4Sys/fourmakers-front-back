namespace DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao
{
    public class SubstituirDadosAlocacaoesPorPeriodoParam
    {
        public string CodigoProjetoNovo { get; set; }
        public string CodigoColaboradorNovo { get; set; }
        public bool EhTbd { get; set; }
        public long[] PeriodosIdSubstituidos { get; set; }
    }
}