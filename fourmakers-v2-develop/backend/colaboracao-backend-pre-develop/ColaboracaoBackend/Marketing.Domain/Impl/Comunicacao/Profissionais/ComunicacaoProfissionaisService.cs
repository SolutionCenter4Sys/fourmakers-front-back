using ApiClient.Domain;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Profissionais;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Profissionais;
using Marketing.Domain.Interfaces.Comunicacao.Profissionais;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Profissionais
{
    public class ComunicacaoProfissionaisService : IComunicacaoProfissionaisService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoProfissionaisRepository _repository;

        public ComunicacaoProfissionaisService(IComunicacaoProfissionaisRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiGenericResult<List<ProfissionalDTO>>> ObterListaProfissionaisAsync(int orgId, string codigoColaboradorUsuarioLogado)
        {
            var result = new ApiGenericResult<List<ProfissionalDTO>>();
            try
            {
                result.Retorno = await _repository.ObterListaProfissionaisAsync(orgId, codigoColaboradorUsuarioLogado);

                var baseUrl = (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE) ?? "").TrimEnd('/');
                foreach (var p in result.Retorno)
                {
                    if (!string.IsNullOrEmpty(p.CodigoColaboradorInterno) && !string.IsNullOrEmpty(baseUrl))
                        p.UrlPerfil = $"{baseUrl}/colaborador/{p.CodigoColaboradorInterno}";
                }
                // Favoritados primeiro, depois ordem por nome
                result.Retorno = result.Retorno.OrderByDescending(p => p.Favoritado).ThenBy(p => p.NomeCompleto).ToList();
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> FavoritarProfissionalAsync(string codigoInternoColaboradorProfissional, string codigoColaboradorUsuarioLogado, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                await _repository.FavoritarProfissionalAsync(codigoColaboradorUsuarioLogado, codigoInternoColaboradorProfissional, orgId);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult> DesfavoritarProfissionalAsync(string codigoInternoColaboradorProfissional, string codigoColaboradorUsuarioLogado, int orgId)
        {
            var result = new ApiGenericResult();
            try
            {
                await _repository.DesfavoritarProfissionalAsync(codigoColaboradorUsuarioLogado, codigoInternoColaboradorProfissional, orgId);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<List<string>>> ListarProfissionaisFavoritadosAsync(string codigoColaboradorUsuarioLogado, int orgId)
        {
            var result = new ApiGenericResult<List<string>>();
            try
            {
                result.Retorno = await _repository.ListarCodigosProfissionaisFavoritadosAsync(codigoColaboradorUsuarioLogado, orgId);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }
    }
}
