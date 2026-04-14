using Competencia.Domain.Enums;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill
{
    public interface IGestorExternoPerfilSkillService
    {
        Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> ListarSkillsPerfilGestorExternoPorId(Guid gestorExternoPerfilId);
        Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> InserirGestorExternoPerfilSkill(GestorExternoPerfilSkillInput gestorExternoPerfilSkillInput, Guid gestorExternoPerfilId, string cpfRequest);
        Task<ApiGenericResult> DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId);
        Task<AdicionarCompetenciaDTO> InserirCompetenciaAPartirDeUmPerfil(string descricao, TipoCompetenciaSRSEnum competencia, string cpf);
    }
}