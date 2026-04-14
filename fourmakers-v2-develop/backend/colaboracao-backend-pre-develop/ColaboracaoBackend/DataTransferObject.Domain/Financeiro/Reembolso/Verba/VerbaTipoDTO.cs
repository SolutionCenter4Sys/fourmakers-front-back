using Newtonsoft.Json;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaTipoDTO
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Label  { get; set; }
    [JsonIgnore]
    public string Operacao { get; set; }
    public string DescricaoAcao { get; set;  }
    public VerbaTipoCustoEnum TipoCodigo { get; set; }
}