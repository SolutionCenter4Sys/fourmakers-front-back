using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Financeiro.IntegracaoBancaria;
using Core.Domain.Reembolso.ControleDeSaldo;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RemessaBancaria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;

namespace Financeiro.Domain.Impl.IntegracaoBancaria.RemessaBancaria
{
    public class RemessaBancariaService : IRemessaBancariaService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Remessa Bancária";
        
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly ICnabOrgRepository _cnabOrgRepository;
        private readonly IPagamentoCnabRepository _pagamentoCnabRepository;
        private readonly ICnabRemessaRepository _cnabRemessaRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly GerarCnab240Service _gerarCnab240Service;
        private readonly INotaFiscalRepository _notaFiscalRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;

        public RemessaBancariaService(
            IDBConnectionUnitOfWork dbConnectionUnitOfWork,
            ICnabOrgRepository cnabOrgRepository,
            IPagamentoCnabRepository pagamentoCnabRepository,
            ICnabRemessaRepository cnabRemessaRepository,
            IUploadFilesClient uploadFilesClient, 
            INotaFiscalRepository notaFiscalRepository, 
            IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService)
        {
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _cnabOrgRepository = cnabOrgRepository;
            _pagamentoCnabRepository = pagamentoCnabRepository;
            _cnabRemessaRepository = cnabRemessaRepository;
            _uploadFilesClient = uploadFilesClient;
            _notaFiscalRepository = notaFiscalRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _gerarCnab240Service = new GerarCnab240Service();
        }

        public async Task<ApiGenericResult<ListarRemessasCnabResult>> ProcessarRemessaBancariaCNAB(
            GerarRemessaBancariaRequest request,
            string cpfRequest,
            int orgId)
        {
            var apiGenericResult = new ApiGenericResult<ListarRemessasCnabResult>();
            string remessaId = null;

            _dbConnectionUnitOfWork.BeginTransaction();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                if (request.SolicitacoesPagamentoIds == null || !request.SolicitacoesPagamentoIds.Any())
                {
                    throw new ArgumentException("É necessário selecionar ao menos uma solicitação de pagamento.");
                }

                // 1. Buscar e preparar dados
                var todasSolicitacoes = await BuscarSolicitacoesPagamento(orgId, request.CodDiretoria, request.TipoRemessa, cpfRequest);

                // 1.1. Filtrar apenas as solicitações selecionadas
                var solicitacoes = todasSolicitacoes.Where(s => request.SolicitacoesPagamentoIds.Contains(s.Id)).ToList();

                if (!solicitacoes.Any())
                {
                    throw new Exception("Não há solicitações de pagamento aguardando processamento para os IDs selecionados.");
                }

                var pagamentosAgrupados = AgruparPagamentosPorColaborador(solicitacoes);
                var pagamentosComDados = await BuscarDadosBancariosColaboradores(pagamentosAgrupados, orgId);

                var solicitacoesAgrupadasPorMetodoPagamento = pagamentosComDados.GroupBy(x => x.DadosBancarios?.FormaPagamento);

                var cnabOrgList = new List<CnabOrgResult>();
                
                foreach (var solicitacao in solicitacoesAgrupadasPorMetodoPagamento)
                {
                    var formaPagamento = solicitacao.Key;
                    cnabOrgList.Add(await BuscarCnabOrg(orgId, request.CodDiretoria, formaPagamento));
                }
                
                var remessasIds = new List<string>();

                foreach (var solicitacao in solicitacoesAgrupadasPorMetodoPagamento)
                {
                    var formaPagamento = solicitacao.Key;
                    
                    // 2. Validar e buscar CNAB Org
                    CnabOrgResult? cnabOrg = null;
                    
                    if(!string.IsNullOrWhiteSpace(request.CodDiretoria))
                    {
                        cnabOrg = cnabOrgList.Where(x => x.FormaPagamento == formaPagamento && x.CodDiretoria == request.CodDiretoria).FirstOrDefault();
                    }
                    else
                    {
                        cnabOrg = cnabOrgList.Where(x => x.FormaPagamento == formaPagamento).FirstOrDefault();
                    }

                    if (cnabOrg == null)
                    {
                        throw new Exception("Configuração CNAB não encontrado");
                    }

                    var solicitacoesGrupo = solicitacao.ToList();

                    // 3. Criar remessa no banco
                    var (hashRemessa, nomeArquivo, dataGeracao) = await GerarDadosRemessa(orgId);
                    var valorTotal = solicitacoesGrupo.Sum(p => p.ValorTotal);
                    remessaId = await CriarRemessa(hashRemessa, nomeArquivo, request.TipoRemessa, request.CodDiretoria, cpfRequest, orgId, cnabOrg, valorTotal);

                    // 4. Gerar linhas do arquivo CNAB240
                    var linhasCnab = _gerarCnab240Service.GerarArquivoCnab240(cnabOrg, solicitacoesGrupo, dataGeracao);

                    // 5. Persistir itens e criar pagamentos
                    await PersistirItensEPagamentos(remessaId, linhasCnab, cnabOrg, solicitacoesGrupo, orgId, request.TipoRemessa);

                    // 5.1. Atualizar status das solicitações para ENVIADO_PARA_PAGAMENTO_CNAB
                    await AtualizarStatusSolicitacoes(solicitacoesGrupo, request.TipoRemessa);

                    // 6. Upload do arquivo para AWS
                    var urlArquivo = await FazerUploadArquivo(linhasCnab, nomeArquivo);

                    // 6.1 Atualizar URL arquivo no banco de dados
                    await _cnabRemessaRepository.AtualizarNomeArquivo(remessaId, urlArquivo, cpfRequest);

                    // 7. Finalizar remessa
                    await _cnabRemessaRepository.AtualizarStatusRemessa(remessaId, "ARQUIVO GERADO", cpfRequest);
                    remessasIds.Add(remessaId);
                }
               
                // 8. Commit da transação
                _dbConnectionUnitOfWork.Commit();

                // 9. Retornar lista completa de remessas (ordenadas por data DESC, recém-criada fica no topo)
                apiGenericResult.Retorno = await _cnabRemessaRepository.ListarRemessas(orgId, tipoRemessa: request.TipoRemessa, mesAnoProcessamento: null, status: null, ids: remessasIds);
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                TratarErro(remessaId, cpfRequest, ex);
            }

            return apiGenericResult;
        }

        private async Task<List<SolicitacaoRemessaDTO>> BuscarSolicitacoesPagamento(int orgId, string codDiretoria, string tipoRemessa, string cpfRequest)
        {
            await ValidaAcessoModuloRemessaCNAB(tipoRemessa, orgId, cpfRequest);
            // 1. Buscar todas as solicitações
            var todasSolicitacoes = await _cnabRemessaRepository.BuscarSolicitacoesPagamentos(orgId, tipoRemessa);
            
            var solicitacoesFiltradas = todasSolicitacoes.AsEnumerable();

            // Filtro por diretoria (se não for vazio/null, filtra; caso contrário, traz todas)
            if (!string.IsNullOrWhiteSpace(codDiretoria))
            {
                solicitacoesFiltradas = solicitacoesFiltradas.Where(s => s.CodDiretoria == codDiretoria);
            }
            
            return solicitacoesFiltradas.ToList();
        }

        private List<ColaboradorPagamentoAgrupadoDTO> AgruparPagamentosPorColaborador(
            List<SolicitacaoRemessaDTO> solicitacoes)
        {
            return solicitacoes
                .GroupBy(s => s.CodigoColaborador)
                .Select(grupo => new ColaboradorPagamentoAgrupadoDTO
                {
                    CodigoInternoColaborador = grupo.Key,
                    SolicitacaoPagamentoIds = grupo.Select(s => s.Id).Distinct().ToList(),
                    SolicitacaoValores = grupo
                        .GroupBy(s => s.Id)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Sum(x => x.ValorParaPagamento)
                        ),
                    ValorTotal = grupo.Sum(s => s.ValorParaPagamento)
                })
                .ToList();
        }

        private async Task<List<ColaboradorPagamentoAgrupadoDTO>> BuscarDadosBancariosColaboradores(
            List<ColaboradorPagamentoAgrupadoDTO> pagamentos,
            int orgId)
        {
            var pagamentosComDados = new List<ColaboradorPagamentoAgrupadoDTO>();

            foreach (var pagamento in pagamentos)
            {
                var dadosBancarios = await _pagamentoCnabRepository.BuscarDadosBancariosColaborador(
                    pagamento.CodigoInternoColaborador,
                    orgId);

                pagamento.DadosBancarios = dadosBancarios;
               
                pagamentosComDados.Add(pagamento);
            }

            return pagamentosComDados;
        }

        private async Task<DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg.CnabOrgResult> BuscarCnabOrg(
            int orgId,
            string codDiretoria,
            string formaPagamento)
        {
            var cnabOrg = await _cnabOrgRepository.ObterCnabOrgPorChaveUnicaAsync(orgId, codDiretoria, formaPagamento);

            if (cnabOrg == null)
            {
                throw new Exception($"Não foi encontrado CNAB Org para a diretoria '{codDiretoria}' e forma de pagamento '{formaPagamento}'.");
            }

            return cnabOrg;
        }

        private async Task<(string hashRemessa, string nomeArquivo, DateTime dataGeracao)> GerarDadosRemessa(int orgId)
        {
            var numeroSequencial = await _cnabRemessaRepository.ObterProximoNumeroSequencial(orgId);
            var hashRemessa = numeroSequencial.ToString("D10"); // Formato com 10 dígitos: 0000000001
            var dataGeracao = DateTime.Now;
            var dataFormatada = DateTimeUtil.ObterDataComUnderscoreParaNomeArquivo(dataGeracao);
            var nomeArquivo = $"REMESSA_CNAB_{dataFormatada}_{hashRemessa}.txt";

            return (hashRemessa, nomeArquivo, dataGeracao);
        }

        private async Task<string> CriarRemessa(
            string hashRemessa,
            string nomeArquivo,
            string tipoRemessa,
            string codDiretoria,
            string cpfRequest,
            int orgId,
            DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg.CnabOrgResult cnabOrg,
            decimal valorTotal)
        {
            return await _cnabRemessaRepository.CriarRemessa(
                hashRemessa,
                nomeArquivo,
                "CNAB240_ITAU_V085",
                tipoRemessa,
                codDiretoria ?? "",
                "GERANDO ARQUIVO",
                cpfRequest,
                orgId,
                cnabOrg.CodigoBanco,
                cnabOrg.Agencia,
                cnabOrg.AgenciaDv,
                cnabOrg.Conta,
                cnabOrg.ContaDV,
                cnabOrg.CodigoConvenio,
                $"Remessa {tipoRemessa} - {DateTime.Now:MM/yyyy}",
                valorTotal);
        }

        private async Task PersistirItensEPagamentos(
            string remessaId,
            List<string> linhasCnab,
            DataTransferObject.Domain.Financeiro.IntegracaoBancaria.CnabOrg.CnabOrgResult cnabOrg,
            List<ColaboradorPagamentoAgrupadoDTO> pagamentos,
            int orgId,
            string tipoRemessa)
        {
            int numeroSequencialRegistro = 1;
            int indicePagamento = 0;

            // Persistir todas as linhas
            foreach (var linha in linhasCnab)
            {
                // Identificar tipo de registro baseado na posição 8 (índice 7)
                var tipoRegistro = ObterTipoRegistroDescritivo(linha);

                var itemId = await _cnabRemessaRepository.CriarRemessaItem(remessaId, linha, numeroSequencialRegistro, tipoRegistro);

                // Se for linha de Segmento A (posição 14 = 'A'), criar pagamento CNAB
                if (linha.Length >= 14 && linha[13] == 'A' && indicePagamento < pagamentos.Count)
                {
                    var pagamento = pagamentos[indicePagamento];

                    var pagamentoCnabId = await _pagamentoCnabRepository.CriarColaboradorPagamentoCnab(
                        orgId,
                        pagamento.CodigoInternoColaborador,
                        itemId,
                        pagamento.ValorTotal,
                        pagamento.DadosBancarios.FormaPagamento,
                        pagamento.DadosBancarios.CodigoBanco ?? "",
                        pagamento.DadosBancarios.Agencia ?? "",
                        pagamento.DadosBancarios.AgenciaDv ?? "",
                        pagamento.DadosBancarios.Conta ?? "",
                        pagamento.DadosBancarios.ContaDv ?? "",
                        pagamento.DadosBancarios.ChavePix ?? "",
                        pagamento.DadosBancarios.TipoChavePix ?? "",
                        "AGUARDANDO");

                    // Criar relações com as solicitações
                    foreach (var solicitacaoId in pagamento.SolicitacaoPagamentoIds)
                    {
                        await _pagamentoCnabRepository.CriarRelacaoComSolicitacaoPagamento(pagamentoCnabId, solicitacaoId, pagamento.SolicitacaoValores[solicitacaoId], tipoRemessa);
                    }

                    indicePagamento++;
                }

                numeroSequencialRegistro++;
            }
        }

        private string ObterTipoRegistroDescritivo(string linha)
        {
            if (linha.Length < 8)
                return "DESCONHECIDO";

            var tipoRegistro = linha.Substring(7, 1);

            return tipoRegistro switch
            {
                "0" => "HEADER_ARQUIVO",
                "1" => "HEADER_LOTE",
                "3" => $"DETALHE_SEGMENTO_{(linha.Length >= 14 ? linha.Substring(13, 1) : "?")}",
                "5" => "TRAILER_LOTE",
                "9" => "TRAILER_ARQUIVO",
                _ => "DESCONHECIDO"
            };
        }

        private async Task AtualizarStatusSolicitacoes(List<ColaboradorPagamentoAgrupadoDTO> pagamentos, string tipoRemessa)
        {
            foreach (var pagamento in pagamentos)
            {
                foreach (var solicitacaoId in pagamento.SolicitacaoPagamentoIds)
                {
                    if (tipoRemessa == "REEMBOLSO")
                    {
                        await _pagamentoCnabRepository.AtualizarStatusSolicitacaoPagamento(
                            int.Parse(solicitacaoId),
                            StatusSolicitacaoPagamentoEnum.ENVIADO_PARA_PAGAMENTO_CNAB);
                    }

                    if (tipoRemessa == "NF")
                    {
                        await _notaFiscalRepository.MudarStatusNotaFiscalPorListaDeIdsAsync([Guid.Parse(solicitacaoId)],
                            NotaFiscalStatusEnum.PROCESSAMENTO_CNAB, null, null);
                    }
                }
            }
        }

        private async Task<string> FazerUploadArquivo(List<string> linhasCnab, string nomeArquivo)
        {
            var conteudoArquivo = string.Join("\r\n", linhasCnab);
            var bytesArquivo = Encoding.UTF8.GetBytes(conteudoArquivo);

            var pathArquivoS3 = $"arquivos/cnab/remessa/{nomeArquivo}";
            await _uploadFilesClient.UploadFile(pathArquivoS3, bytesArquivo);

            return $"{VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA)}{pathArquivoS3}";
        }

        private void TratarErro(string remessaId, string cpfRequest, Exception ex)
        {
            // O rollback da transação já reverte todos os inserts
            // Não precisa marcar como CANCELADO pois o registro será removido pelo rollback
            ExceptionUtil.GerenciarRetornoExcecao(new ApplicationException($"Erro ao executar {DESCRICAO_ENTIDADE}. Detalhe erro: {ex.Message}"), CRUDEnum.Create, DESCRICAO_ENTIDADE);
        }

        private void ValidaAcessoCnabOrg(string cpfRequest, int orgId)
        {
            //var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
            //    cpfRequest, orgId, FuncionalidadeSistemaEnum.CADASTRO_XPTO);

            //if (!isValid.Result)
            //{
            //    throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}");
            //}
        }

        public async Task<ApiGenericResult<ListarSolicitacoesPagamentoCNABResult>> ListarSolicitacoesPagamentoCNAB(
            string tipoRemessa,
            string codDiretoria,
            string cpfRequest,
            int orgId)
        {
            var apiGenericResult = new ApiGenericResult<ListarSolicitacoesPagamentoCNABResult>();

            try
            {
                ValidaAcessoCnabOrg(cpfRequest, orgId);

                // Reutiliza o mesmo método de filtragem usado no ProcessarRemessaBancariaCNAB
                var solicitacoes = await BuscarSolicitacoesPagamento(orgId, codDiretoria, tipoRemessa, cpfRequest);
                var pagamentosAgrupados = AgruparPagamentosPorColaborador(solicitacoes);
                var pagamentosComDados = await BuscarDadosBancariosColaboradores(pagamentosAgrupados, orgId);

                // Agrupa por colaborador com solicitações discriminadas e forma de pagamento
                var colaboradoresAgrupados = pagamentosComDados
                    .Select(pagamento => new ColaboradorComSolicitacoesDTO
                    {
                        CodigoColaborador = pagamento.CodigoInternoColaborador,
                        NomeColaborador = solicitacoes.First(s => s.CodigoColaborador == pagamento.CodigoInternoColaborador).Nome,
                        QuantidadeSolicitacoes = pagamento.SolicitacaoPagamentoIds.Count,
                        ValorTotal = pagamento.ValorTotal,
                        FormaPagamento = pagamento.DadosBancarios?.FormaPagamento,
                        Solicitacoes = solicitacoes.Where(s => pagamento.SolicitacaoPagamentoIds.Contains(s.Id)).ToList()
                    })
                    .ToList();

                apiGenericResult.Retorno = new ListarSolicitacoesPagamentoCNABResult
                {
                    Colaboradores = colaboradoresAgrupados,
                    TotalSolicitacoes = colaboradoresAgrupados.Sum(c => c.QuantidadeSolicitacoes),
                    TotalColaboradores = colaboradoresAgrupados.Count,
                    ValorTotal = colaboradoresAgrupados.Sum(c => c.ValorTotal)
                };

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(
                    new ApplicationException($"Erro ao listar solicitações de pagamento CNAB. Detalhe erro: {ex.Message}"),
                    CRUDEnum.Read,
                    DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ListarRemessasCnabResult>> ListarRemessasCnab(
            int orgId,
            string tipoRemessa,
            string mesAnoProcessamento = null,
            string status = null)
        {
            var apiGenericResult = new ApiGenericResult<ListarRemessasCnabResult>();

            try
            {
                var resultado = await _cnabRemessaRepository.ListarRemessas(orgId, tipoRemessa, mesAnoProcessamento, status);
                apiGenericResult.Retorno = resultado;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(
                    new ApplicationException($"Erro ao listar remessas CNAB. Detalhe erro: {ex.Message}"),
                    CRUDEnum.Read,
                    DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<List<string>>> BuscarModulosRemessaOrg(int orgId, string codigoInternoColaborador)
        {
            var apiResult = new ApiGenericResult<List<string>>();
            var reembolso = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.REEMBOLSO_HABILITAR_REMESSA_CNAB, orgId, codigoInternoColaborador);
            var nf = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.NOTA_FISCAL_HABILITAR_REMESSA_CNAB, orgId, codigoInternoColaborador);

            var remessasPermitidas = new List<string>();

            if (reembolso)
            {
                remessasPermitidas.Add("REEMBOLSO");
            }
            
            if (nf)
            {
                remessasPermitidas.Add("NF");
            }
            
            apiResult.Retorno = remessasPermitidas;;

            return apiResult;
        }

        public async Task ValidaAcessoModuloRemessaCNAB(string tipoRemessa, int orgId, string cpfRequest)
        {
            var resultado = await BuscarModulosRemessaOrg(orgId, tipoRemessa);
            if (!resultado.Retorno.Contains(tipoRemessa))
            {
                throw new Exception($"CNAB não habilitado para {tipoRemessa}");
            }
        }
    }
}
