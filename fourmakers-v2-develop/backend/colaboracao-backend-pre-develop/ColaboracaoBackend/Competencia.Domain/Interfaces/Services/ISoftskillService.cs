using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface ISoftskillService
    {
        SoftskillDTO AdicionarSoftSkill(string descricao, string cpfRequest, bool processamentoLote = false);
        List<SoftskillDTO> ListarSoftSkill(string busca, int cursor, int limite);
        ListaSoftskillResult ListarSoftSkillsNaoAtribuidas(string busca, int cursor, int limite, string cpfRequest);
        SoftskillDTO GetSoftskillById(long id);
        List<SoftskillColaboradorDTO> ListarSoftskillColaborador(string cpfColaborador);
        SoftskillColaboradorDTO RemoveSoftskillColaborador(long softskillId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        SoftskillColaboradorDTO AddSoftskillColaborador(long softskillId, long? nivelId, string cpfRequest, string token, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<ItemPerfilResult> AdicionarSoftskillColaboradorEmLote(List<AddSoftskillColabParam> param, string token, bool minhaJornada, string usuarioLogado);
        List<NivelDTO> ListarNivelSoftskill();
        SoftskillColaboradorDTO AlterarSoftskillColaborador(string cpf, long id, long? nivelId, string gestorExternoPerfil, bool minhaJornada, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL);
        List<long> ListarSoftSkillsAtribuidas(string cpfColaborador);
        Task<List<KeyValuePair<string, long>>> GetSoftSkillInfoByDescricao(List<string> softSkills);
        Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupSoftSkillByIds(List<long> ids);

    }
}