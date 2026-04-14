namespace DataTransferObject.Domain.CCH
{
    public class ColaboradorCCH
    {
        public int cdProfissional { get; set; }
        public string nmProfissional { get; set; }
        public int flFuncionarioAtivo { get; set; }
        public string statusFuncionario { get; set; }
        public string cdCpf { get; set; }
        public string nmLogin { get; set; }
        public string nmEnderecoEletronico { get; set; }
        public long cdCargo { get; set; }
        public string nmCargo { get; set; }
        public int cdDivisao { get; set; }
        public string nmDivisao { get; set; }
    }
}