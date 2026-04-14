using Competencia.Domain.Enums;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IGestorExternoPerfilSkillRepository
    {
        Task<IEnumerable<GestorExternoPerfilSkillResult>> ListarGestorExternoPerfilSkillsPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId);
        Task<IEnumerable<GestorExternoPerfilSkillResult>> InserirGestorExternoPerfilSkillAsync(GestorExternoPerfilSkillInput parametroRepositoryInput);
        Task<bool> DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId);
        Task<bool> ValidarRelacaoNivelItemPerfilAsync(long itemPerfilId, long nivelId);
        Task<bool> ValidarSkillParaItemPerfilAsync(long skillId, long itemPerfilId);

        Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf);
    }
}