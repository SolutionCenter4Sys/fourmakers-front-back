using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.Holerite;
using Core.Domain.Financeiro.IntegracaoBancaria;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using DataTransferObject.Domain.Util.Enum;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.Externo;
using System.Collections.Generic;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.Externo
{
    public class IntegracaoBancariaExternaService : IIntegracaoBancariaExternaService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Integração Bancária Externa";

        private readonly ICnabRetornoRepository _cnabRetornoRepository;
        private readonly IHoleriteRepository _holeriteRepository;
        private readonly ITokenSistemaService _tokenSistemaService;

        public IntegracaoBancariaExternaService(
            ICnabRetornoRepository cnabRetornoRepository,
            IHoleriteRepository holeriteRepository,
            ITokenSistemaService tokenSistemaService)
        {
            _cnabRetornoRepository = cnabRetornoRepository;
            _holeriteRepository = holeriteRepository;
            _tokenSistemaService = tokenSistemaService;
        }

        public async Task<ApiGenericResult<List<ReembolsoPagoExternoDTO>>> BuscarReembolsosPagosViaCNAB(
            string tokenSistema)
        {
            int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);

            var apiGenericResult = new ApiGenericResult<List<ReembolsoPagoExternoDTO>>();

            try
            {
                var reembolsos = await _cnabRetornoRepository.BuscarReembolsosPagosViaCNAB(orgId);
                apiGenericResult.Retorno = reembolsos;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(
                    new ApplicationException($"Erro ao buscar reembolsos pagos. Detalhe erro: {ex.Message}"),
                    CRUDEnum.Read,
                    DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<HoleriteLiquidoConciliadoExternoDTO>>> ListarHoleritesLiquidosConciliacaoFolhaPontoAsync(
            string tokenSistema,
            int mes,
            int ano,
            string codDiretoria)
        {
            var result = new ApiGenericResult<List<HoleriteLiquidoConciliadoExternoDTO>>();

            if (mes < 1 || mes > 12)
            {
                result.Sucesso = false;
                result.Mensagem = "Mês inválido. Informe um valor entre 1 e 12.";
                return result;
            }

            if (ano < 2000 || ano > 2100)
            {
                result.Sucesso = false;
                result.Mensagem = "Ano inválido.";
                return result;
            }

            try
            {
                int orgId = _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURSYS_2);
                string codDiretoriaFiltro = string.IsNullOrWhiteSpace(codDiretoria) ? null : codDiretoria.Trim();

                var linhas = await _holeriteRepository.ListarHoleritesLiquidosConciliacaoFolhaPontoAprovadaAsync(
                    orgId,
                    mes,
                    ano,
                    codDiretoriaFiltro);

                result.Retorno = linhas ?? new List<HoleriteLiquidoConciliadoExternoDTO>();
                result.Sucesso = true;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(
                    new ApplicationException($"Erro ao listar holerites com conciliação de folha ponto. Detalhe erro: {ex.Message}"),
                    CRUDEnum.Read,
                    DESCRICAO_ENTIDADE);
            }

            return result;
        }
    }
}
