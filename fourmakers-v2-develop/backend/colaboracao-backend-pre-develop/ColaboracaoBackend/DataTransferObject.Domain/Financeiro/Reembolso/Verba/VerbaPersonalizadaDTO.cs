using System.Text.Json.Serialization;
using DataTransferObject.Domain.Colaborador;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaPersonalizadaDTO
{
    public int Id { get; set; }
    public bool Ativo { get; set; }
    public bool CustoCliente { get; set; }
    public decimal Valor { get; set; }
    [JsonIgnore]
    public string ProjetoId { get; set; } = string.Empty;
    [JsonIgnore]
    public string ClienteId { get; set; } = string.Empty;
    public SimpleVerbaDTO Verba { get; set; }
    public SimpleVerbaTipoDTO VerbaTipo { get; set; }
}

public class SimpleVerbaDTO
{
    public int Id { get; set; }
    public string Categoria  { get; set; } = string.Empty;
}

public class SimpleVerbaTipoDTO
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
}