using System.Globalization;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Financeiro.IntegracaoContabil;
using Core.Domain.Reembolso.ControleDeSaldo;
using Core.Domain.Reembolso.Solicitacao;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Financeiro.Domain.Interfaces.Reembolso.Validadores;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Colaboracao.Helper.Extension;
using Core.Domain.Financeiro.Banco;
using DataTransferObject.Domain.Fourmakers;
using Foursys.Domain.Interfaces.Services;
using K4os.Hash.xxHash;
using Logs.Infra.Attributes;
using Microsoft.IdentityModel.Tokens;


namespace Financeiro.Domain.Services.Reembolso.Solicitacao;

[LogDomainClass]
public class SolicitacaoReembolsoService : ISolicitacaoReembolsoService
{
    private readonly ISolicitacaoReembolsoRepository _solicitacaoReembolsoRepository;
    private readonly IDBConnectionUnitOfWork _unitOfWork;
    private readonly ISolicitacaoReembolsoValidadorService _solicitacaoReembolsoValidadorService;
    private readonly ISolicitacaoStatusService _solicitacaoStatusService;
    private readonly ISolicitacaoReembolsoDocumentoService _solicitacaoReembolsoDocumentoService;
    private readonly IIAClient _iaCLient;
    private readonly IControleDeSaldoRepository _controleDeSaldoRepository;
    private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
    private readonly IIntegracaoContabilRepository _integracaoContabilRepository;
    private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
    private readonly IDadosBancariosColaboradorRepository _dadosBancariosColaboradorRepository;
    private readonly ITokenFileTempRepository _tokenFileTempRepository;

    public SolicitacaoReembolsoService(ISolicitacaoReembolsoRepository solicitacaoReembolsoRepository,
                                       IDBConnectionUnitOfWork unitOfWork,
                                       ISolicitacaoReembolsoValidadorService solicitacaoReembolsoValidadorService,
                                       ISolicitacaoStatusService solicitacaoStatusService,
                                       ISolicitacaoReembolsoDocumentoService solicitacaoReembolsoDocumentoService,
                                       IIAClient iaCLient,
                                       IControleDeSaldoRepository controleDeSaldoRepository,
                                       IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                       IIntegracaoContabilRepository integracaoContabilRepository, 
                                       IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService, 
                                       IDadosBancariosColaboradorRepository dadosBancariosColaboradorRepository,
                                       ITokenFileTempRepository tokenFileTempRepository)
    {
        _solicitacaoReembolsoRepository = solicitacaoReembolsoRepository;
        _unitOfWork = unitOfWork;
        _solicitacaoReembolsoValidadorService = solicitacaoReembolsoValidadorService;
        _solicitacaoStatusService = solicitacaoStatusService;
        _solicitacaoReembolsoDocumentoService = solicitacaoReembolsoDocumentoService;
        _iaCLient = iaCLient;
        _controleDeSaldoRepository = controleDeSaldoRepository;
        _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        _integracaoContabilRepository = integracaoContabilRepository;
        _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
        _dadosBancariosColaboradorRepository = dadosBancariosColaboradorRepository;
        _tokenFileTempRepository = tokenFileTempRepository;
    }

    public async Task<ApiGenericResult<SolicitacaoColaboradorDetalhesDTO>> ListarPorColabAsync(string tokenUsuario, string codigoInternoColaborador, int orgId, DateTime? dataInicial, DateTime? dataFinal)
    {
        var apiGenericResult = new ApiGenericResult<SolicitacaoColaboradorDetalhesDTO>();
    
        try
        {
            var solicitacoes = await _solicitacaoReembolsoRepository.ListarPorColabAsync(codigoInternoColaborador, orgId, dataInicial, dataFinal);
            decimal totalSolicitado = solicitacoes.Sum(x => x.ValorSolicitado);
            decimal? totalAprovado = solicitacoes.Where(x => x.ValorAprovado != null).Sum(x => x.ValorAprovado);
            decimal saldoColaborador = await _controleDeSaldoRepository.BuscarSaldoColaborador(codigoInternoColaborador, orgId);
            var souAprovador = await _solicitacaoReembolsoRepository.VerificaSeEhAprovador(codigoInternoColaborador, orgId);
            var souGestorAdm = await _solicitacaoReembolsoRepository.VerificaSeEhGestorAdm(codigoInternoColaborador, orgId);
            
            var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenUsuario));

            foreach (var solicitacao in solicitacoes)
            {
                foreach (var documento in solicitacao.SolicitacaoDocumentos)
                {
                    var pathUrl = documento.Url;
                    var replaceUrl = pathUrl.Replace("$1", base64Token);
                    documento.Url = replaceUrl;
                }
            }
            
            var agrupador = solicitacoes
                .GroupBy(x => new 
                {
                    x.Objetivo,
                    x.Destino,
                    x.DataInicio,
                    x.DataFim,
                    x.ClienteDescricao,
                    x.ProjetoDescricao,
                    Data = x.Data.Date
                })
                .Select(g => new AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoColaboradorDTO>>
                {
                    NomeColaborador = "",
                    Objetivo = g.Key.Objetivo,
                    Destino = g.Key.Destino,
                    Periodo = (g.Key.DataInicio == null || g.Key.DataFim == null)
                        ? ""
                        : $"{g.Key.DataInicio:dd/MM/yyyy} - {g.Key.DataFim:dd/MM/yyyy}",
                    Cliente = g.Key.ClienteDescricao,
                    Projeto = g.Key.ProjetoDescricao,
                    DataSolicitacao = g.Key.Data,
                    SomaValores = g.Sum(x => x.ValorSolicitado),
                    Objeto = g.ToList() // <<--- lista dos itens agrupados
                })
                .ToList();
            
            var result = new SolicitacaoColaboradorDetalhesDTO()
            {
                Solicitacoes = agrupador,
                TotalAprovado = totalAprovado?? 0,
                TotalSolicitado = totalSolicitado,
                Saldo = saldoColaborador,
                SouAprovador = souAprovador || souGestorAdm,
                SouGestor = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(codigoInternoColaborador, orgId, FuncionalidadeSistemaEnum.PARAMETRIZACAO_REEMBOLSO).Result
            };

            apiGenericResult.Retorno = result;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Solicitacoes");
        }
        
        return apiGenericResult;
    }

    /// <summary>
    /// Cria solicitação(ões) em transação. Para cada anexo com <c>TokenArquivoTemp</c>, grava o documento reutilizando a chave S3 já enviada no upload (sem rename).
    /// Ao final, remove os tokens de <c>tb_token_files_temp</c> com <see cref="ITokenFileTempRepository.ExcluirPorTokensAsync"/> para não expurgar o objeto na <c>RotinaLimpezaAws</c>.
    /// </summary>
    /// <remarks>Ver <c>docs/reembolso-arquivos-temporarios-s3.md</c>.</remarks>
    public async Task<ApiGenericResult> InserirAsync(InserirSolicitacaoReembolsoListaDTO parametro, int orgId, string codigoInternoColaborador)
    {
        var apiGenericResult = new ApiGenericResult();
        _unitOfWork.BeginTransaction();

        try
        {
            //Validando a data de comprovante de todas as solicitações, retornando uma lista, caso tiver uma lista preenchida impedir de continuar e retornar apenas a lista de erros
            var errosValidacao = await _solicitacaoReembolsoValidadorService.GerarListaDeErrosReferenteAComprovante(parametro.Solicitacoes, orgId);
            var buscaParametroObrigatoriedadeMetodoPagamento = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoFrontEndEnum.REEMBOLSO_COLETA_DADOS_BANCARIOS, orgId, codigoInternoColaborador);
            if (buscaParametroObrigatoriedadeMetodoPagamento)
            {
                var buscaMetodoUsuario =
                    await _dadosBancariosColaboradorRepository.BuscarDadosBancariosPorColaboradorIdAsync(
                        codigoInternoColaborador, orgId);
                if (buscaMetodoUsuario == null)
                {
                    errosValidacao.Add("Para solicitar um reembolso é preciso incluir pelo menos 1 metodo de pagamento.");
                }
            }
            
            if (errosValidacao.Count > 0)
            {
                apiGenericResult.Sucesso = false;
                apiGenericResult.Erros = errosValidacao;
                return apiGenericResult;
            }
            
            _solicitacaoReembolsoValidadorService.ValidarAgrupador(parametro);

            var tokensConsumidos = new List<string>();
            foreach (var solicitacao in parametro.Solicitacoes)
            {
                await _solicitacaoReembolsoValidadorService.ValidarSolicitacao(solicitacao, CRUDEnum.Create, orgId, codigoInternoColaborador);
                var statusList = await _solicitacaoStatusService.ListarAsync(orgId, codigoInternoColaborador);
                var statusPadrao = statusList.Retorno.First(x => x.Descricao == "Pendente");
                var insertId = await _solicitacaoReembolsoRepository.InserirAsync(orgId, solicitacao.ProjetoId, solicitacao.ClienteId, solicitacao.VerbaId, codigoInternoColaborador, solicitacao.Descricao, solicitacao.DataDespesa, solicitacao.Valor, solicitacao.ValorUnidade, solicitacao.Quantidade, statusPadrao.Id, parametro.Objetivo, parametro.Destino, parametro.DataInicio, parametro.DataFim);

                foreach (var arquivo in solicitacao.Arquivos)
                {
                    if (arquivo.TokenArquivoTemp.IsNullOrEmpty()) continue;
                    var (_, token) = await _solicitacaoReembolsoDocumentoService.InserirDocumentoAsync(arquivo, codigoInternoColaborador, insertId);
                    tokensConsumidos.Add(token);
                }
            }
            await _tokenFileTempRepository.ExcluirPorTokensAsync(tokensConsumidos);
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Solicitacao");
        }
        return apiGenericResult;
    }

    public async Task<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoGerenteDTO>>>>> ListarSolicitacoesPendentes(ListarSolicitacoesGerenteProjetoParam param, string codColaborador, int orgId, string tokenUsuario)
    {
        var apiResult = new ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoGerenteDTO>>>>();
        try
        {
            var buscaResult = await _solicitacaoReembolsoRepository.ListarSolicitacoesGerenteProjeto(param.Filtro, param.ClienteId, param.ProjetoId, param.DataInicio, param.DataFim, codColaborador, orgId, param.StatusId);
            
            var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenUsuario));
            
            foreach (var solicitacao in buscaResult)
            {
                foreach (var documento in solicitacao.SolicitacaoDocumentos)
                {
                    var pathUrl = documento.Url;
                    var replaceUrl = pathUrl.Replace("$1", base64Token);
                    documento.Url = replaceUrl;
                }
            }
            
            var agrupador = buscaResult
                .GroupBy(x => new 
                {
                    x.Colaborador,
                    x.Objetivo,
                    x.Destino,
                    x.DataInicio,
                    x.DataFim,
                    x.ClienteDescricao,
                    x.ProjetoDescricao,
                    DataSolicitacao = x.DataSolicitacao.Date
                })
                .Select(g => new AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoGerenteDTO>>
                {
                    NomeColaborador = g.Key.Colaborador,
                    Objetivo = g.Key.Objetivo,
                    Destino = g.Key.Destino,
                    Periodo = (g.Key.DataInicio == null || g.Key.DataFim == null)
                        ? ""
                        : $"{g.Key.DataInicio:dd/MM/yyyy} - {g.Key.DataFim:dd/MM/yyyy}",
                    Cliente = g.Key.ClienteDescricao,
                    Projeto = g.Key.ProjetoDescricao,
                    DataSolicitacao = g.Key.DataSolicitacao,
                    SomaValores = g.Sum(x => x.Valor.ParseDecimalUniversal()),
                    Objeto = g.ToList() // <<--- lista dos itens agrupados
                })
                .ToList();
            
            apiResult.Retorno = agrupador;
            
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Solicitacao");
        }
        return apiResult;
    }

    public async Task<ApiGenericResult<SolicitacaoBigNumberDTO>> SolicitacaoBigNumbers(string codColaborador, int orgId)
    {
        var apiResult = new ApiGenericResult<SolicitacaoBigNumberDTO>();
        try
        {
            apiResult.Retorno = await _solicitacaoReembolsoRepository.SolicitacaoBigNumbersAsync(codColaborador, orgId);
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Solicitacao");
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorAprovacaoDTO>>>>> ListarSolicitacoesAprovacaoPorColaborador(string tokenUsuario, string codGerente, string codColaborador, int orgId, string? clienteId, string? projetoId)
    {
        var apiResult = new ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorAprovacaoDTO>>>>();
        try
        {
            var solicitacoes = await _solicitacaoReembolsoRepository.ListarSolicitacoesAprovacaoPorColaborador(clienteId, projetoId, codGerente, codColaborador, orgId);

            // Codifica o token em Base64
            var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenUsuario));

            foreach (var solicitacao in solicitacoes)
            {
                foreach (var documento in solicitacao.SolicitacaoDocumentos)
                {
                    var pathUrl = documento.Url;
                    var replaceUrl = pathUrl.Replace("$1", base64Token);
                    documento.Url = replaceUrl;
                }
            }
            
            var agrupador = solicitacoes
                .GroupBy(x => new 
                {
                    x.Colaborador,
                    x.Objetivo,
                    x.Destino,
                    x.DataInicio,
                    x.DataFim,
                    x.ClienteDescricao,
                    x.ProjetoDescricao,
                    DataSolicitacao = x.DataSolicitacao.Date
                })
                .Select(g => new AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorAprovacaoDTO>>
                {
                    NomeColaborador = g.Key.Colaborador,
                    Objetivo = g.Key.Objetivo,
                    Destino = g.Key.Destino,
                    Periodo = (g.Key.DataInicio == null || g.Key.DataFim == null)
                        ? ""
                        : $"{g.Key.DataInicio:dd/MM/yyyy} - {g.Key.DataFim:dd/MM/yyyy}",
                    Cliente = g.Key.ClienteDescricao,
                    Projeto = g.Key.ProjetoDescricao,
                    DataSolicitacao = g.Key.DataSolicitacao,
                    SomaValores = g.Sum(x => x.Valor),
                    Objeto = g.ToList() // <<--- lista dos itens agrupados
                })
                .ToList();
            
            apiResult.Retorno = agrupador;

        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Solicitacao");
        }
        return apiResult;
    }
    
    public async Task<ApiGenericResult> AprovarSolicitacoes(List<int> solicitacoesId, string codAprovador, int orgId)
    {
        var apiResult = new ApiGenericResult();
        _unitOfWork.BeginTransaction();
        try
        {
            var temParametrizacaoReembolso = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(codAprovador, orgId, FuncionalidadeSistemaEnum.PARAMETRIZACAO_REEMBOLSO);
            var temCFO = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoFrontEndEnum.REEMSOLBO_WORKFLOW_APROVACAO_CFO, orgId, codAprovador);
            foreach (var id in solicitacoesId)
            {
                
                var solicitacao = await _solicitacaoReembolsoValidadorService.ValidarAprovacao(id, codAprovador, null, SolicitacaoStatusEnum.Aprovado, orgId);
                var statusAprovacao = SolicitacaoStatusEnum.Aprovado;
                
                if (!temParametrizacaoReembolso && !solicitacao.EhGestorProjeto)
                {
                    throw new InvalidOperationException(
                        "Solicitação não pode ser aprovada: usuário não possui parametrização e não é gestor."
                    );
                }

                if (temParametrizacaoReembolso)
                {
                    statusAprovacao = SolicitacaoStatusEnum.Aprovado;
                }
                else if (solicitacao.EhGestorProjeto)
                {
                    statusAprovacao = temCFO
                        ? SolicitacaoStatusEnum.AprovadoGestor
                        : SolicitacaoStatusEnum.Aprovado;
                }

                if (statusAprovacao == SolicitacaoStatusEnum.Aprovado)
                {
                    var status = await GerarSolicitacaoDePagamento(solicitacao.Id, solicitacao.CodigoColaborador, solicitacao.Valor, solicitacao.OrgId);
                    if (status == SolicitacaoStatusEnum.Pago)
                    {
                        statusAprovacao = status;
                    }
                }
                
                await _solicitacaoReembolsoRepository.AprovarSolicitacao(id, codAprovador, solicitacao.Valor, statusAprovacao);
                
            }
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Solicitacao");
            throw;
        }
        
        return apiResult;
    }

    private async Task<SolicitacaoStatusEnum> GerarSolicitacaoDePagamento(int solicitacaoId, string codigoInternoColaborador, decimal valor, int orgId)
    {
        var saldoAtual = await _controleDeSaldoRepository.BuscarSaldoColaborador(codigoInternoColaborador, orgId);
        var status = SolicitacaoStatusEnum.Aprovado;
        
        var tipoDaOperacao = await _solicitacaoReembolsoRepository.BuscarOperacaoDaSolicitacao(solicitacaoId);
        if (tipoDaOperacao == "+")
        {
            // Gera Adiantamento como PAGO
            await InserirSolicitacaoDePagamento(solicitacaoId, codigoInternoColaborador, orgId, valor, saldoAtual, StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO, TipoMovimentacaoPagamentoEnum.ENTRADA, false);
        }
        else
        {
            if (saldoAtual == 0)
            {
                // Gerar "AGUARDANDO PAGAMENTO" para o valor total
                await InserirSolicitacaoDePagamento(solicitacaoId, codigoInternoColaborador, orgId, valor, saldoAtual, StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO, TipoMovimentacaoPagamentoEnum.SAIDA, false);
            }
            else if (saldoAtual - valor < 0)
            {
                // Saldo parcial: abater o que conseguir
                var valorPago = saldoAtual;
                var valorRestante = valor - saldoAtual;

                // Gera "PAGO" com valor parcial
                await InserirSolicitacaoDePagamento(solicitacaoId, codigoInternoColaborador, orgId, valorPago, saldoAtual, StatusSolicitacaoPagamentoEnum.PAGO, TipoMovimentacaoPagamentoEnum.SAIDA, true);

                // Gera "AGUARDANDO PAGAMENTO" com o valor restante
                await InserirSolicitacaoDePagamento(solicitacaoId, codigoInternoColaborador, orgId, valorRestante, saldoAtual, StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO, TipoMovimentacaoPagamentoEnum.SAIDA, false);
            }
            else
            {
                // Saldo suficiente: registrar como "PAGO"
                status = SolicitacaoStatusEnum.Pago;
                await InserirSolicitacaoDePagamento(solicitacaoId, codigoInternoColaborador, orgId, valor, saldoAtual, StatusSolicitacaoPagamentoEnum.PAGO, TipoMovimentacaoPagamentoEnum.SAIDA, true);
            }
        }

        return status;
    }

    private async Task InserirSolicitacaoDePagamento(int solicitacaoId, string codigoColaborador, int orgId, decimal valor, decimal saldo, StatusSolicitacaoPagamentoEnum statusPagamento, TipoMovimentacaoPagamentoEnum tipoPagamento, bool usouSaldo)
    {
        // Criar uma nova solicitação de pagamento, Pago ou Aguardando Pagamento
        var novaSolicitacao = await _controleDeSaldoRepository.CriarSolicitacaoDePagamento(statusPagamento, codigoColaborador, orgId, solicitacaoId, "REEMBOLSO", valor, usouSaldo);
        if (statusPagamento == StatusSolicitacaoPagamentoEnum.PAGO)
        {
            // Gera o novo saldo calculado baseado no tipo de movimentacao
            var calculoSaldo = tipoPagamento == TipoMovimentacaoPagamentoEnum.SAIDA ? saldo - valor : saldo + valor;
            await _controleDeSaldoRepository.EditarValorSaldoColaborador(codigoColaborador, orgId, calculoSaldo);
            // Gera o extrato de movimentação
            await _controleDeSaldoRepository.GerarExtratoPagamento(tipoPagamento, novaSolicitacao, orgId, valor, codigoColaborador);
        }
    }
    
    public async Task<ApiGenericResult> ReprovarSolicitacoes(List<int> solicitacoesId, string observacao, string codAprovador, int orgId)
    {
        var apiResult = new ApiGenericResult();
        _unitOfWork.BeginTransaction();
        try
        {
            foreach (var id in solicitacoesId)
            {
                await _solicitacaoReembolsoValidadorService.ValidarAprovacao(id, codAprovador, observacao, SolicitacaoStatusEnum.Reprovado, orgId);
                await _solicitacaoReembolsoRepository.ReprovarSolicitacao(id, codAprovador, observacao);
            }
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Solicitacao");
            throw;
        }
        
        return apiResult;
    }

    public async Task<ApiGenericResult<AnaliseDocumentoSolicitacaoTotalizadorDTO>> AnalisarComprovantesFiscais(List<Base64DTO> documentos)
    {
        var apiResult = new ApiGenericResult<AnaliseDocumentoSolicitacaoTotalizadorDTO>();
        try
        {
            if (documentos.Count == 0)
            {
                throw new ArgumentException("Lista de comprovantes vazia.");
            }
            var token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN");

            var listaDeRetornos = new List<AnaliseDocumentoSolicitacaoDTO>();
            foreach (var documento in documentos)
            {
                if (documento.Tipo != ArquivoTipoEnum.pdf)
                {
                    var busca = await _iaCLient.AnalisarDocumentoIA(documento.Base64, token);
                    listaDeRetornos.Add(busca);
                }
            }

            var objetoRetorno = new AnaliseDocumentoSolicitacaoTotalizadorDTO
            {
                Analises = listaDeRetornos,
                Data = listaDeRetornos
                    .OrderByDescending(x => x.Data)
                    .Select(x => x.Data)
                    .FirstOrDefault(),
                Quantidade = listaDeRetornos
                    .SelectMany(x => x.Itens)
                    .Sum(item => item.Quantidade),
                Valor = listaDeRetornos
                    .SelectMany(x => x.Itens)
                    .Sum(item => item.Valor)
            };
            
            apiResult.Retorno = objetoRetorno;
            
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Documentos");
            throw;
        }

        return apiResult;
    }

    public async Task<ApiGenericResult<SolicitacaoColaboradorVisaoAdmResult>> ListarSolicitacoesVisaoAdm(SolicitacaoColaboradorVisaoAdmParam param, int orgId, string tokenUsuario)
    {
        var apiResult = new ApiGenericResult<SolicitacaoColaboradorVisaoAdmResult>();
        try
        {
            var select = await _solicitacaoReembolsoRepository.ListarSolicitacoesVisaoAdm(orgId, param.CodigoCliente, param.CodigoProjeto, param.DataInicio, param.DataFim, param.AprovadorId, param.StatusId);
            var solicitacoesPagamento = await _controleDeSaldoRepository.BuscarSolicitacoesPagamentos(orgId);
            var selectIds = select.Select(x => x.Id).ToList();

            var filtroSolicitacoes = solicitacoesPagamento.Where(x => selectIds.Contains(x.ReembolsoId)).ToList();
            var sumValorAprovado = filtroSolicitacoes.Where(x => !x.SaldoAbatido).Sum(x => x.Valor);
            var sumValorPago = filtroSolicitacoes.Where(x => !x.SaldoAbatido && x.Status == StatusSolicitacaoPagamentoEnum.PAGO).Sum(x => x.Valor);
            
            var quantidadeColaborador = select.DistinctBy(x => x.CodigoColaborador).Count();
            var valorSolicitado = select.Sum(x => x.ValorSolicitado);
            var valorReprovado = select.Where(x => x.StatusId == (int)SolicitacaoStatusEnum.Reprovado).Sum(x => x.ValorSolicitado);
            var valorAprovado = sumValorAprovado;
            var valorPendente = select.Where(x => x.StatusId == (int)SolicitacaoStatusEnum.Pendente).Sum(x => x.ValorSolicitado);
            var valorPago = sumValorPago;
            
            var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenUsuario));

            foreach (var solicitacao in select)
            {
                foreach (var documento in solicitacao.SolicitacaoDocumentos)
                {
                    var pathUrl = documento.Url;
                    var replaceUrl = pathUrl.Replace("$1", base64Token);
                    documento.Url = replaceUrl;
                }
            }

            var codigosColaboradores = select.Select(x => x.CodigoColaborador).Distinct().ToList();
            var saldosPorColaborador = await _controleDeSaldoRepository.BuscarSaldosColaboradores(codigosColaboradores, orgId);

            var agrupador = select
                .GroupBy(x => new 
                {
                    x.NomeColaborador,
                    x.Objetivo,
                    x.Destino,
                    x.DataInicio,
                    x.DataFim,
                    x.Cliente,
                    x.Projeto,
                    DataSolicitacao = x.DataSolicitacao.Date 
                })
                .Select(g => new AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorVisaoAdmDTO>>
                {
                    NomeColaborador = g.Key.NomeColaborador,
                    SaldoAdiantamentos = saldosPorColaborador.GetValueOrDefault(g.First().CodigoColaborador, 0),
                    Objetivo = g.Key.Objetivo,
                    Destino = g.Key.Destino,
                    Periodo = (g.Key.DataInicio == null || g.Key.DataFim == null)
                        ? ""
                        : $"{g.Key.DataInicio:dd/MM/yyyy} - {g.Key.DataFim:dd/MM/yyyy}",
                    Cliente = g.Key.Cliente,
                    Projeto = g.Key.Projeto,

                    // A data agora está normalizada
                    DataSolicitacao = g.Key.DataSolicitacao,

                    SomaValores = g.Sum(x => x.ValorSolicitado),
                    Objeto = g.ToList()
                })
                .ToList();

            apiResult.Retorno = new()
            {
                Colaboradores = quantidadeColaborador,
                Solicitacoes = agrupador,
                ValorAprovado = valorAprovado,
                ValorSolicitado = valorSolicitado,
                ValorPendente = valorPendente,
                ValorReprovado = valorReprovado,
                ValorPago = valorPago
            };
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, "Solicitacoes");
            throw;
        }
        return apiResult;
    }

    public async Task<ApiGenericResult<FileContentResult>> GerarReleatorioSolicitacoesAguardandoPagamento(int orgId, string cpfRequest)
    {
        var apiResult = new ApiGenericResult<FileContentResult>();
        _unitOfWork.BeginTransaction();
        try
        {
            var solicitacoes = await _controleDeSaldoRepository.BuscarSolicitacoesPagamentoPorStatusRelatorio(StatusSolicitacaoPagamentoEnum.AGUARDANDO_PAGAMENTO, orgId);

            var excelDynamicResult = solicitacoes.Select(x => new
            {
                x.CodigoColaborador,
                x.Nome,
                x.Cliente,
                x.Projeto,
                x.DataDaDespesa,
                x.DataDoPedido,
                x.DataDaAprovacao,
                x.NomeDoAprovador,
                x.ValorSolicitado,
                x.ValorAprovado,
                x.ValorAbatidoDoSaldo,
                x.ValorParaPagamento,
                CustoCliente = x.CustoCliente? "SIM" : "NÃO",
                x.FormaPagamento
            }).ToList();
            
            var fileBytes = ExcelFileUtil.CreateExcelFile(excelDynamicResult);
            var fileName = "Exportacao_Relatorio_Pagamento_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

            apiResult.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            apiResult.Retorno.FileDownloadName = fileName;

            // await GerarPagamentos(solicitacoes, orgId, cpfRequest);
            
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Relatorio");
            throw;
        }
        
        return apiResult;
    }
    
    public async Task GerarPagamentoDeSolicitacoesPorIds(List<int> ids, int orgId, string cpfRequest)
    {
        try
        {
            var solicitacoes =
                await _controleDeSaldoRepository.BuscarSolicitacoesPagamentoPorIds(ids, orgId);
            await GerarPagamentos(solicitacoes, orgId, cpfRequest);
            
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Pagamentos do reembolso");
            throw;
        }
    }

    public async Task<ApiGenericResult> GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIdsAsync(List<int> ids, int orgId, string cpfRequest)
    {
        var apiGenericResult = new ApiGenericResult();
        try
        {
            _unitOfWork.BeginTransaction();
            var temAcesso = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpfRequest, orgId, FuncionalidadeSistemaEnum.PARAMETRIZACAO_REEMBOLSO);
            if (!temAcesso)
            {
                throw new UnauthorizedAccessException("Acesso não autorizado para pagamentos de reembolso");
            }
            var solicitacoes =
                await _controleDeSaldoRepository.BuscarSolicitacoesPagamentoPorListaDeSolicitacaoDeReembolsoIds(ids, orgId);
            await GerarPagamentos(solicitacoes, orgId, cpfRequest);
            _unitOfWork.Commit();
        }
        catch (Exception e)
        {
            _unitOfWork.SafeRollback();
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Create, "Pagamentos do reembolso");
            throw;
        }

        return apiGenericResult;
    }

    private async Task GerarPagamentos(List<SolicitacaoPagamentoRelatorioDTO> solicitacoes, int orgId, string cpfRequest)
    {
        foreach (var solicitacao in solicitacoes)
        {
            var Tipo = solicitacao.Operacao == "+" ? TipoMovimentacaoPagamentoEnum.ENTRADA : TipoMovimentacaoPagamentoEnum.SAIDA;
            if (Tipo == TipoMovimentacaoPagamentoEnum.ENTRADA)
            {
                var saldoColaborador = await _controleDeSaldoRepository.BuscarSaldoColaborador(solicitacao.CodigoColaborador, orgId);
                var novoSaldo = saldoColaborador + solicitacao.ValorParaPagamento;
                await _controleDeSaldoRepository.EditarValorSaldoColaborador(solicitacao.CodigoColaborador, orgId, novoSaldo);
            }

            await _solicitacaoReembolsoRepository.PagarSolicitacao(solicitacao.SolicitacaoReembolsoId, cpfRequest, SolicitacaoStatusEnum.Pago);

            await _controleDeSaldoRepository.GerarExtratoPagamento(Tipo, solicitacao.Id, orgId, solicitacao.ValorParaPagamento, solicitacao.CodigoColaborador);
            await _controleDeSaldoRepository.EditarStatusPagamento(solicitacao.Id, StatusSolicitacaoPagamentoEnum.PAGO, false);
        }
    }
}