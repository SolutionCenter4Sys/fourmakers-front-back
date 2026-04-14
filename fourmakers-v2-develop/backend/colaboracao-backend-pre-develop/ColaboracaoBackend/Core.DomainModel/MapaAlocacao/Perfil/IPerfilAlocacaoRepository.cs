using DataTransferObject.Domain;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;

namespace Core.Domain
{
    public interface IPerfilAlocacaoRepository
    {
        List<PerfilAlocacaoDTO> ListarPerfilAlocacao(string codProjeto, int orgId, bool ocultarSkills = false);
        List<SkillNivelDTO> ListarSkillsPerfilAlocacao(string perfilId);
        List<SkillNivelDTO> ListarSkillsAlocacao(long idAlocacao);
        PerfilAlocacaoDTO BuscarPerfilAlocacao(long codAlocacao);
        void VincularPerfilAlocacao(long idAlocacao, string codPerfilGestorExterno, string codPerfil, int orgId);
        void AlteraVinculoPerfilAlocacao(long idAlocacao, string codPerfilGestorExterno, string codPerfil);
        string InserirNovoPerfilAlocacao(string perfil, string codProjeto, List<ItemSkillPerfilAlocacaoDTO> skills, string codInternoColaborador, int orgId);
        void InserirSkillsAlocacao(long idAlocacao, List<ItemSkillPerfilAlocacaoDTO> skills, string codInternoColaborador);
        void RemoverSkillAlocacao(long idAlocacao, ItemSkillPerfilAlocacaoDTO skill);
        string ObterCodClientePorPerfilId(string perfilId, int orgId);
    }
}