using Competencia.Domain.Enums;

namespace Competencia.API.DTOs;

public class EditarNomeCompetenciaParam
{
    public string NomeCompetencia { get; set; }
    public string NovoNomeCompetencia { get; set; }
    public TipoCompetenciaSRSEnum TipoCompetencia { get; set; }
}