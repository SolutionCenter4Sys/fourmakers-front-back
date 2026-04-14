using DataTransferObject.Domain.Base;

namespace DataTransferObject.Domain.Competencia;

public class VwSkillColaboradorDTO
{
    public int Id { get; set; }
    public string Descricao { get; set; }
    public int NivelId { get; set; }
    public string Tipo { get; set; }
    public string Nivel { get; set; }
    public string CodigoInternoColaborador { get; set; }
}