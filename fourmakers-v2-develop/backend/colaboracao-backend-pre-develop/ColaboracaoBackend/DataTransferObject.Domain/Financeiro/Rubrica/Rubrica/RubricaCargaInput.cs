using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;

public class RubricaCargaInput
{
    public Base64DTO Base64File { get; set; }
    public string RubricaId { get; set; }
    public int MesInicial { get; set; }
    public int AnoFinal { get; set; }
    public string CodDiretoria  { get; set; }
}