using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Extension;
using SRS.Infra.Constantes;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl
{
    [LogDomainClass]
    public class ExtracaoAlocacaoService : IExtracaoAlocacaoService
    {
        private readonly IExtracaoAlocacaoRepository _extracaoAlocacaoRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IMapaAlocacaoRepository _mapaAlocacaoRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public ExtracaoAlocacaoService(IExtracaoAlocacaoRepository extracaoAlocacaoRepository,
                                       IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                       IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService, IMapaAlocacaoRepository mapaAlocacaoRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _extracaoAlocacaoRepository = extracaoAlocacaoRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _mapaAlocacaoRepository = mapaAlocacaoRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioAlocacoes(string cpf, int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                ValidaAcessoExtracaoAlocacao(cpf, orgId, FuncionalidadeSistemaEnum.RELATORIO_ALOCACOES);

                var extracaoAlocacaoResult = await _extracaoAlocacaoRepository.ExportarAlocacoesTodasExcel(orgId);

                if (!extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existe alocações para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);
                var fileName = "Exportacao_Alocacoes_Todas_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioAlocacoesPorPesquisa(ListarAlocacoesColabETbdInput dto, string cpfSolicitante, int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                ValidaAcessoExtracaoAlocacao(cpfSolicitante, orgId, FuncionalidadeSistemaEnum.RELATORIO_ALOCACOES);
                string qtdGerenteProjetoPrioridade = "1";

                var incluiInativos = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_INCLUIR_INATIVOS_MAPA_ALOCACAO, orgId, cpfSolicitante);

                var restricaoDeAcesso =
                    await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfSolicitante,
                        orgId, RestricaoDeAcessoTipoConstants.DIRETORIA, dto.CodigoUnidade);
                var (alocacoes, perfis) = await _mapaAlocacaoRepository.ListarAlocacoesColaboradoresETbdsDynamicAsync(dto.Pesquisa.ToNullSeTextoNullOuZero(),
                                                                                                                 null,
                                                                                                                 orgId,
                                                                                                                 restricaoDeAcesso,
                                                                                                                 dto.CodigoDepartamento.ToNullSeTextoNullOuZero(),
                                                                                                                 dto.CodigoGestorAdm.ToNullSeTextoNullOuZero(),
                                                                                                                 dto.ListaCodigoColabOuTbd,
                                                                                                                 dto.FiltroTipoProfissional,
                                                                                                                 dto.CodigoGestorProjeto.ToNullSeTextoNullOuZero(),
                                                                                                                 dto.ListaCodigoClientes,
                                                                                                                 dto.ApenasProjetosPrioritarios,
                                                                                                                 dto.ListaCodigoProjetos,
                                                                                                                 dto.CodigoStatusProjeto.ToNullSeTextoNullOuZero(),
                                                                                                                 qtdGerenteProjetoPrioridade,
                                                                                                                 incluiInativos,
                                                                                                                 null,
                                                                                                                 null
                                                                                                                );

                if (!alocacoes.Any())
                {
                    ret.Mensagem = "Não existe alocações para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(alocacoes);
                var fileName = "Exportacao_Alocacoes_Filtradas_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        private void ValidaAcessoExtracaoAlocacao(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para Extracao de Alocação");
            }
        }
    }
}