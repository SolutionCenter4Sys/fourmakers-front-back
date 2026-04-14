namespace DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

public class DadosBancariosColaboradorBase
{
    public string CodigoBancoTed { get; set; } = null; //CHAR(3) NULLABLE
    public string AgenciaTed { get; set; } = null; //CHAR(5) NULLABLE
    public string AgenciaDvTed { get; set; } = null; // CHAR(1) NULLABLE
    public string ContaTed { get; set; } = null; // CHAR(12) NULLABLE
    public string ContaDvTed { get; set; } = null; // CHAR(1) NULLABLE
    public string ChavePix { get; set; } = null; // VARCHAR(100) NULLABLE
    public string TipoChavePix { get; set; } = null; // VARCHAR(1) NULLABLE - 'C'=CPF, 'J'=CNPJ, 'E'=e-mail, 'T'=telefone, 'R'=EVP
    public FormaPagamentoEnum FormaPagamento { get; set; } // PIX - BANCO
}
