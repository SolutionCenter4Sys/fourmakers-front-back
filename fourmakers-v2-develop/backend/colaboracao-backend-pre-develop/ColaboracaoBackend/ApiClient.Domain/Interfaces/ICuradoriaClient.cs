using System.Collections.Generic;
using System.Threading.Tasks;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Curadoria;

namespace ApiClient.Domain.Interfaces;

public interface ICuradoriaClient
{
    Task<CuradoriaSkillResponse> ObterComparativoSkill(string request, TipoCompetenciaSRSEnum tipoCompetencia);
}