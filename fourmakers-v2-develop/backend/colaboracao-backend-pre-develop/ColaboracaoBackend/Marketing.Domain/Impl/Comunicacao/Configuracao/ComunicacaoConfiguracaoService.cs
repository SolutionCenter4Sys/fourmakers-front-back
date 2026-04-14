using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Configuracao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Configuracao;
using Marketing.Domain.Interfaces.Comunicacao.Configuracao;
using System;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Configuracao
{
    public class ComunicacaoConfiguracaoService : IComunicacaoConfiguracaoService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoConfiguracaoRepository _repository;

        public ComunicacaoConfiguracaoService(IComunicacaoConfiguracaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiGenericResult<ConfiguracaoNotificacaoDTO>> InserirOuAtualizarConfiguracaoAsync(string codigoInternoColaborador, int orgId, ConfiguracaoNotificacaoDTO request)
        {
            var result = new ApiGenericResult<ConfiguracaoNotificacaoDTO>();
            try
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                result.Retorno = await _repository.InserirOuAtualizarConfiguracaoAsync(codigoInternoColaborador, orgId, request);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<ConfiguracaoNotificacaoDTO>> ObterConfiguracaoAsync(string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<ConfiguracaoNotificacaoDTO>();
            try
            {
                var config = await _repository.ObterConfiguracaoAsync(codigoInternoColaborador, orgId);
                
                if (config == null)
                {
                    var configuracaoPadrao = new ConfiguracaoNotificacaoDTO
                    {
                        NotificaWeb = true,
                        NotificaTeams = false,
                        NotificaEmail = true,
                        NotificaPlataforma = true
                    };
                    
                    result.Retorno = await _repository.InserirOuAtualizarConfiguracaoAsync(codigoInternoColaborador, orgId, configuracaoPadrao);
                }
                else
                {
                    result.Retorno = config;
                }
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }
    }
}
