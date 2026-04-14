namespace DataTransferObject.Domain.Apontamento;

public class ApontamentoExcedenteDTO
{
    public string NomeColaborador  { get; set; }
    public int HorasTotais  { get; set; }
    public int HorasAprovadas  { get; set; }
    public int HorasPendentes  { get; set; }
}