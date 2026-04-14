using System;
using DataTransferObject.Domain.Util.Enum;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaMassivaItemLogDTO
{
    public int Id { get; set; }
    public string CodigoCargaRubrica { get; set; }
    public string JsonItemTentativa { get; set; }
    public StatusProcessamentoItemEnum StatusProcessamento { get; set; }
    public TipoIdentificacaoEnum? TipoIdentificacao { get; set; }
    public string CodigoInternoColaborador { get; set; }
    public string TbRubricaColaboradorId { get; set; }
    public string MensagemErro { get; set; }
    public DateTime DataProcessamento { get; set; }
}