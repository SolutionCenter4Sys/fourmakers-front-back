namespace DataTransferObject.Domain.Fourmakers.ParametroConfiguracao
{
    public class ParametroConfiguracaoBase
    {
        public string ColaboradorOrgCpf { get; set; }
        public int? GrupoAcessoId { get; set; }
        public string CodigoParametro { get; set; }
        public string ValorParametro { get; set; }
        public int ParametroNivelId { get; set; }
    }
}