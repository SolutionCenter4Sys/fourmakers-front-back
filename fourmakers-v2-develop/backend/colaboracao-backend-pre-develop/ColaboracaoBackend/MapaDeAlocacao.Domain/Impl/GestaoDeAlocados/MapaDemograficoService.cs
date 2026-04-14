using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.MapaDemografico;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados
{
    [LogDomainClass]
    public class MapaDemograficoService : IMapaDemograficoService
    {

        private readonly IMapaDemograficoRepository _mapaDemograficoRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public MapaDemograficoService(IMapaDemograficoRepository mapaDemograficoRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _mapaDemograficoRepository = mapaDemograficoRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<ApiGenericResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosOrgId(string cpfRequest, int? orgID = null, string talento = null)
        {
            try
            {
                var restricoesDiretoria = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgID,  RestricaoDeAcessoTipoConstants.DIRETORIA);
                var list = await _mapaDemograficoRepository.BuscarBancoTalentosOrgId(restricoesDiretoria, orgID, talento);
                return list == null ? new ApiGenericResult<List<MapaDemograficoDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Banco de Talentos não encontrados."

                } : new ApiGenericResult<List<MapaDemograficoDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Banco de Talentos encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Banco de Talentos: {e.Message}");
            }
        }

        public async Task<ApiGenericResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosCodColaborador(string codColaborador)
        {
            try
            {
                var list = await _mapaDemograficoRepository.BuscarBancoTalentosCodColaborador(codColaborador);
                return list == null ? new ApiGenericResult<List<MapaDemograficoDTO>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Banco de Talentos não encontrados."

                } : new ApiGenericResult<List<MapaDemograficoDTO>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Banco de Talentos encontrados. total: {list.Count}"
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Erro ao listar Banco de Talentos: {e.Message}");
            }
        }
        
        public async Task<ApiGenericResult<List<MapaDemograficoPcdSumarioDTO>>> ListarSumarioPcdsAsync(int orgId)
        {
            var apiResult = new ApiGenericResult<List<MapaDemograficoPcdSumarioDTO>>();
            try
            {
                var retornoBusca = await _mapaDemograficoRepository.ListarSumarioPcdsAsync(orgId);
                apiResult.Retorno = retornoBusca;

            }
            catch (Exception e)
            {
                ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Sumario Pcds");
            }

            return apiResult;
        }
    }
}
