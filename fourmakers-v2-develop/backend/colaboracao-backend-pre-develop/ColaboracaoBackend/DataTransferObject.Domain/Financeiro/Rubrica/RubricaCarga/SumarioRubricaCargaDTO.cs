namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class SumarioRubricaCargaDTO
{
    public string RubricaId { get; set; }
    public int MesInicial { get; set; }
    public int AnoFinal { get; set; }
    public string CodDiretoria  { get; set; }
}