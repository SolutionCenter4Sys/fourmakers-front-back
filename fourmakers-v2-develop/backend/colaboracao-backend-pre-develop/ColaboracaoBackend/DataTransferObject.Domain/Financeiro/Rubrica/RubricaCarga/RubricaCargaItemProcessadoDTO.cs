namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaItemProcessadoDTO
{
    public int NumeroLinha { get; set; }
    public string Vigencia { get; set; }
    public string NomeColaborador { get; set; }
    public string CPF { get; set; }
    public string CodRubrica { get; set; }
    public string DescricaoRubrica { get; set; }
    public string ValorGerado { get; set; }
    public string Status { get; set; } // "Sucesso", "Erro", "Duplicado", "Atualizado"
    public string MensagemStatus { get; set; }
    public string IdRubricaColaborador { get; set; } // ID gerado/atualizado (se houver)
}