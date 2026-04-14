using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;

namespace MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados
{
    public interface IMinhaJornadaService
    {
        Task<ApiGenericResult<List<MinhaJornadaColaboradorDTO>>> BuscarSkillColaborador(string codColaborador);

        Task<ApiGenericResult<List<MinhaJornadaDTO>>> BuscarSkillColaboradorAlocado(string codColaborador, int orgId);

        Task<ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>> InserirSugestao(SugestaoParamDTO param, bool minhaJornada, string cpfUsuarioLogado);
        Task<ApiGenericResult<List<SugestaoHistoricoDTO>>> AprovarRejeitarSugestao(SugestaoHistoricoParamDTO param);
        Task<ApiGenericResult<SugestaoSkillResponseDTO>> AtualizarSugestao(SugestaoAtualizacaoParamDTO param);
        Task<ApiGenericResult<bool>> DeletarSugestao(string id);
        Task<ApiGenericResult<SugestaoSkillResponseDTO>> BuscarSugestaoPorId(string id);
        Task<ApiGenericResult<List<SugestaoSkilleHistoricoResponseDTO>>> BuscarSugestaoPorCodColaboradorOuAdm(string codInternoColaborador, string codInternoGestor, string perfilId);
        Task<ApiGenericResult<List<SugestaoHistoricoDTO>>> BuscarHistoricoPorId(string sugestaoId);
        Task<ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>> BuscarSugestaoHistoricoPorId(string sugestaoId);
        Task<ApiGenericResult<CandidatosMatchResponse>> CalcularAderenciaDoColaboradorAoPerfil(Guid perfilId, string codigoInternoColaborador, int orgId, string cpfRequest);
    }
};
