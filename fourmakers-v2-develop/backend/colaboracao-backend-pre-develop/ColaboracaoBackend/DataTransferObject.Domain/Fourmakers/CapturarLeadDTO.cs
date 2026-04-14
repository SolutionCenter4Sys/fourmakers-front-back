namespace DataTransferObject.Domain.Fourmakers;

public class CapturarLeadDTO
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string NomeEmpresa { get; set; }
    public CaputraLeadColaboradorQuantidadeEnum OpcaoColaboradorEnum { get; set; }
}

public enum CaputraLeadColaboradorQuantidadeEnum : int
{
    OPCAO_19 = 1,
    OPCAO_20_99 = 2,
    OPCAO_100_499 = 3,
    OPCAO_500 = 4,
}