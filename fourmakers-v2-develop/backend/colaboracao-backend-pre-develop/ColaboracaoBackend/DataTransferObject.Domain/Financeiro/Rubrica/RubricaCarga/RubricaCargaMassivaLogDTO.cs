using System;
using DataTransferObject.Domain.Util.Enum;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class RubricaCargaMassivaLogDTO
{
    public int Id { get; set; }
    public string CodigoCargaRubrica { get; set; }
    public string ArquivoOrigem { get; set; }
    public string CodDiretoria { get; set; }
    public string TbRubricaId { get; set; }
    public int TbRubricaTemplateId { get; set; }
    public StatusProcessamentoCargaEnum StatusProcessamento { get; set; }
    public string MensagemErro { get; set; }
    public int QtdRegistrosProcessadosSucesso { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataProcessamentoFim { get; set; }
    public int QtdRegistrosRetornados { get; set; }
    public int TbOrgId { get; set; }
    public string CodigoInternoColaboradorCriacao { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}