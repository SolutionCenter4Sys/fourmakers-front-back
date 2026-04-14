using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaItemLogDTO
{
    public int Id { get; set; }
    public string CodigoCargaRubrica { get; set; }
    public string JsonItemTentativa { get; set; }
    public string StatusProcessamento { get; set; }
    public string TipoIdentificacao { get; set; }
    public string CodigoInternoColaborador { get; set; }
    public string TbRubricaColaboradorId { get; set; }
    public string MensagemErro { get; set; }
    public DateTime DataProcessamento { get; set; }
}