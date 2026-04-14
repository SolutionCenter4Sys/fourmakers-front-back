namespace DataTransferObject.Domain.Apontamento.ApontamentoValidador.ValidaApontamentoResult
{ 
    public class ValidaApontamentoResult
    {
        public bool EhLancamentoParaOutroColaborador { get; set; }
        public string CpfUtilizado { get; set; }
        public ColaboradorApontamentoDTO Apontamento { get; set; }
    }
}
