namespace DataTransferObject.Domain.Competencia;

public class AlterarNomeCompetenciaResultDTO
{
    public string NomeCompetencia { get; set; }
    public int CompetenciaID { get; set; }
    public string NovoNome { get; set; }
    public int? NovoId { get; set; }
    public string Tipo { get; set; }
    public AlterarTabelasUnificacaoCuradoriaDTO LogsAlteracao { get; set; }
}