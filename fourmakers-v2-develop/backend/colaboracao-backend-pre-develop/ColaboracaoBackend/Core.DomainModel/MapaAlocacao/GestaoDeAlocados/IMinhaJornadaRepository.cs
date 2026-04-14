using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;

namespace Core.Domain.MapaAlocacao.GestaoDeAlocados
{
    public interface IMinhaJornadaRepository
    {
        Task<List<MinhaJornadaColaboradorDTO>> BuscarSkillColaborador(string codColaborador);
        Task<List<MinhaJornadaDTO>> BuscarSkillColaboradorAlocado(string codColaborador, int orgId);
        Task<SugestaoSkilleHistoricoOrdemResponseDTO> InserirSugestao(SugestaoParamDTO param);
        Task<List<SugestaoHistoricoDTO>> AprovarRejeitarSugestao(SugestaoHistoricoParamDTO param);
        Task<SugestaoSkillResponseDTO> AtualizarSugestao(SugestaoAtualizacaoParamDTO param);
        Task<bool> DeletarSugestao(string ID);
        Task<SugestaoSkillResponseDTO> BuscarSugestaoPorId(string Id);
        Task<List<SugestaoSkilleHistoricoResponseDTO>> BuscarSugestaoPorCodColaboradorOuAdm(string codInternoColaborador, string codInternoGestor, string perfilId);
        Task<List<SugestaoHistoricoDTO>> BuscarHistoricoPorId(string sugestaoId);
        Task<SugestaoSkilleHistoricoOrdemResponseDTO> BuscarSugestaoHistoricoPorId(string sugestaoId);
    }
}
