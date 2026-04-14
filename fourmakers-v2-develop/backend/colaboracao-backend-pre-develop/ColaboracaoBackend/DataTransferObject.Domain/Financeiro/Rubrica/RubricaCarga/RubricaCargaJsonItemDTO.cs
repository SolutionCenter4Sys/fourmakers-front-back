using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaJsonItemDTO
{
    [Required(ErrorMessage = "Vigência é obrigatória")]
    public string Vigencia { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "Código da rubrica é obrigatório")]
    public string CodRubrica { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatória")]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "Valor gerado é obrigatório")]
    public string ValorGerado { get; set; }
}