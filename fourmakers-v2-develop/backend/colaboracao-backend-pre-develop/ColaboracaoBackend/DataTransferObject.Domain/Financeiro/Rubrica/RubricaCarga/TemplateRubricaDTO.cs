using System;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class TemplateRubricaDTO
{
    public string Id { get; set; }
    public string NomeTemplate { get; set; }
    public string MapeamentoCampos { get; set; }
    public string LinkModeloS3  { get; set; }
    public string InstrucoesAdicionais  { get; set; }
    public bool Ativo  { get; set; }
    public DateTime DataCriacao { get; set; }
    public string CodigoAlternativaTipo  { get; set; }
}