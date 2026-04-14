using Financeiro.Domain.Interfaces.Conciliacao;
using Core.Domain.Financeiro.Conciliacao;
using Core.Domain.Apontamento;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Financeiro.Conciliacao;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Core.Domain.Financeiro.Holerite;
using DataTransferObject.Domain.Financeiro.Holerite;
using Core.Domain.Reembolso.ControleDeSaldo;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;
using Core.Domain.Financeiro.Rubrica;
using Colaboracao.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Aws.Infra.Interfaces;
using Colaboracao.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Colaboracao.Helper.Util;
using Microsoft.AspNetCore.Mvc;
using DataTransferObject.Domain.Base;
using Colaboracao.Core.Exceptions;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using Core.DomainModel.Projeto;

using Logs.Infra.Attributes;

namespace Financeiro.Domain.Impl.Conciliacao
{
    [LogDomainClass]
    public class ConciliacaoService : IConciliacaoService
    {
        private readonly IConciliacaoFolhaPontoRepository _conciliacaoFolhaPontoRepository;
        private readonly ILoteRepository _loteRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IFolhaPontoRepository _folhaPontoRepository;
        private readonly IHoleriteRepository _holeriteRepository;
        private readonly IControleDeSaldoRepository _controleDeSaldoRepository;
        private readonly IRubricaColaboradorRepository _rubricaColaboradorRepository;
        private readonly IProjetoOrgRepository _projetoOrgRepository;
        private readonly IQueueProducer _queueProducer;
        private readonly IDBConnectionUnitOfWork _unitOfWork;
        public ConciliacaoService(
            IConciliacaoFolhaPontoRepository conciliacaoFolhaPontoRepository,
            ILoteRepository loteRepository,
            IUsuarioColaboradorRepository usuarioColaboradorRepository,
            IFolhaPontoRepository folhaPontoRepository,
            IHoleriteRepository holeriteRepository,
            IControleDeSaldoRepository controleDeSaldoRepository,
            IRubricaColaboradorRepository rubricaColaboradorRepository,
            IQueueProducer queueProducer,
            IDBConnectionUnitOfWork unitOfWork,
            IProjetoOrgRepository projetoOrgRepository)
        {
            _conciliacaoFolhaPontoRepository = conciliacaoFolhaPontoRepository;
            _loteRepository = loteRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _folhaPontoRepository = folhaPontoRepository;
            _holeriteRepository = holeriteRepository;
            _controleDeSaldoRepository = controleDeSaldoRepository;
            _rubricaColaboradorRepository = rubricaColaboradorRepository;
            _queueProducer = queueProducer;
            _unitOfWork = unitOfWork;
            _projetoOrgRepository = projetoOrgRepository;
        }

        public async Task<SumarioConciliacaoResult> CriarLoteConciliacaoAsync(string cnpj, string competencia, int orgId, string codigoInternoSolicitante)
        {
            
            
            var queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("AWS_SQS_QUEUE_ANALISEHOLERITE");
            // Validar se o usuário existe na organização
            var usuario = _usuarioColaboradorRepository.GetUserByCPFEOrgId(codigoInternoSolicitante, orgId);
            if (usuario == null)
            {
                throw new InvalidOperationException($"Usuário com código {codigoInternoSolicitante} não encontrado na organização {orgId}");
            }

            // Validar formato da competência (MM/YYYY)
            if (!ValidarFormatoCompetencia(competencia))
            {
                throw new ArgumentException("Formato de competência inválido. Use o formato MM/YYYY");
            }

            // Validar CNPJ
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                throw new ArgumentException("CNPJ inválido. Deve conter 14 dígitos");
            }

            // Validar se o CNPJ pertence ao projeto da organização
            var cnpjPertenceProjeto = await _conciliacaoFolhaPontoRepository.ValidaCnpjProjetoOrgAsync(cnpj, orgId);
            if (!cnpjPertenceProjeto)
            {
                throw new InvalidOperationException($"CNPJ {cnpj} não pertence ao projeto da organização {orgId}");
            }
            
            var lotesInfo = await _conciliacaoFolhaPontoRepository.BuscarLotesPorOrgAsync(orgId);
            var lotes = lotesInfo.Select(x => { 
                    var ret = JsonConvert.DeserializeObject<SumarioConciliacaoResult>(x.SumarioConciliacaoString);
                    ret.Id = x.Id;
                    return ret;
                }
            ).ToList();
            if (lotes.Count > 0)
            {
                foreach (var lote in lotes)
                {
                    if (lote != null && lote.Cnpj == cnpj && lote.Competencia == competencia )
                    {
                        var loteInfo = lotesInfo.Where(x => x.Id == lote.Id).First();
                        if(loteInfo.DataFinalizacao == null)
                            throw new InvalidOperationException($"Já existe um lote de conciliação para a organização {orgId}");
                    }
                }
            }

            _unitOfWork.BeginTransaction();
            try{
                // Buscar folhas de ponto da organização e competência
                var folhasPonto = await _folhaPontoRepository.BuscarFolhasPontoPorOrgECompetenciaAsync(orgId, cnpj, competencia);

                // Verificar se existem folhas de ponto
                if (folhasPonto.Count == 0)
                {
                    throw new InvalidOperationException($"Não existem folhas de ponto para a competência {competencia} na organização {orgId}");
                }

                var nomeEmpresa = await _conciliacaoFolhaPontoRepository.ObterNomeEmpresaAsync(cnpj, orgId);
                var modalidadePagamentoHorasExtras = await _conciliacaoFolhaPontoRepository.ObterModalidadePagamentoHorasExtrasAsync(cnpj, orgId);

                var partes = competencia.Split('/');
                var mes = int.Parse(partes[0]);
                var ano = int.Parse(partes[1]);
                var listaRubricasColaborador = await _rubricaColaboradorRepository.ListarRubricasColaboradorDetalhadoAsync(mes, ano, orgId);
                
                var listarTodosOsHoleritesDaCompetenciaECnpj = await _holeriteRepository.BuscarHoleritesPorCompetenciaAsync(competencia, cnpj, orgId);
                
                var codigoColaboradorFiltradoHolerite = listarTodosOsHoleritesDaCompetenciaECnpj.Select(x => x.CodigoInternoColaborador).Distinct().ToList();
                
                var colaboradores = folhasPonto.Select(x => x.CodigoInternoColaborador).Distinct().ToList();
                
                var idsNaoEncontradosEmColaboradores = codigoColaboradorFiltradoHolerite
                    .Except(colaboradores)
                    .ToList();

                if (idsNaoEncontradosEmColaboradores.Any())
                {
                    colaboradores.AddRange(idsNaoEncontradosEmColaboradores);
                }
                
                var solicitacoesPagamento = await _controleDeSaldoRepository.BuscarSolicitacoesPagamentoPorStatusRelatorio(StatusSolicitacaoPagamentoEnum.PAGO, orgId, competencia);

                var listaConciliacaoFolhaPontoColaborador = new List<DadosConciliacaoColaboradorDTO>();
                foreach (var colaborador in colaboradores)
                {
                    
                    var existeFolhaPonto = folhasPonto.FirstOrDefault(x => x.CodigoInternoColaborador == colaborador);
                    RelatorioPontoRootDTO? folhaPontoColaborador = null;

                    if (existeFolhaPonto != null)
                    {
                        folhaPontoColaborador = JsonConvert.DeserializeObject<RelatorioPontoRootDTO>(existeFolhaPonto.ObjetoFolhaPontoString);
                    }
                    
                    var objHoleriteColaborador =
                        (await _holeriteRepository.BuscarHoleritesPorColaboradorAsync(colaborador, competencia, cnpj, orgId))
                        .Where(x =>
                            (x.Adiantamento == null || x.Adiantamento == false) &&
                            (x.DecimoTerceiroAdiantamento == null || x.DecimoTerceiroAdiantamento == false) &&
                            (x.DecimoTerceiro == null || x.DecimoTerceiro == false) &&
                            (x.Ferias == null || x.Ferias == false)
                        )
                        .ToList();
                    
                    if (objHoleriteColaborador.Count == 0)
                    {
                        continue;
                    }

                    var holeriteColaborador = JsonConvert.DeserializeObject<HoleriteWrapperDTO>(objHoleriteColaborador.First().ObjetoHoleriteString).Holerite;
                    holeriteColaborador.ConfiguracoesProjeto = new ConfiguracaoProjetoHoleriteDTO() { ModalidadeHE = modalidadePagamentoHorasExtras };
                    
                    var quantidadeDependentes = await _usuarioColaboradorRepository.BuscarQuantidadeDeDependentesPorCodigoInternoColaborador(colaborador);
                    holeriteColaborador.BasesDeCalculo.DependentesIrrf = quantidadeDependentes.ToString();

                    var solicitacaoPagamentoColaborador = solicitacoesPagamento.Where(x => x.CodigoColaborador == colaborador).ToList();
                    
                    var rubricasColaborador = listaRubricasColaborador.Where(x => x.CodigInternoColaborador == colaborador).ToList();
                    listaConciliacaoFolhaPontoColaborador.Add(new DadosConciliacaoColaboradorDTO {
                        Holerite = holeriteColaborador,
                        FolhaPonto = folhaPontoColaborador,
                        Pagamentos = solicitacaoPagamentoColaborador,
                        Rubricas = rubricasColaborador,
                        ItemLoteId = "",
                        CodigoInternoColaborador = colaborador
                    });
                }

                if(listaConciliacaoFolhaPontoColaborador.Count == 0)
                {
                    throw new InvalidOperationException($"Não existem holerites para nenhum colaborador na competência {competencia} na organização {orgId}");
                }

                

                // Criar sumário em JSON para o lote
                var sumario = new SumarioConciliacaoResult
                {
                    Cnpj = cnpj,
                    Competencia = competencia,
                    Empresa = nomeEmpresa,
                    QuantidadeItens = listaConciliacaoFolhaPontoColaborador.Count
                };

                var sumarioJson = JsonConvert.SerializeObject(sumario);

                // Criar o lote
                var loteId = await _loteRepository.CriarLoteAsync(
                    quantidadePaginas: sumario.QuantidadeItens,
                    filePath: "",
                    orgId: orgId,
                    usuarioId: usuario.UsuarioId,
                    tipoFila: TipoFilaEnum.ANALISE_HOLERITE,
                    sumario: sumarioJson
                );

                sumario.Id = loteId;

                // Criar registro de conciliação
                var conciliacaoFolhaPonto = new ConciliacaoFolhaPontoDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    Competencia = competencia,
                    Cnpj = cnpj,
                    TbLoteId = loteId
                };

                await _conciliacaoFolhaPontoRepository.InativarConciliacoesPorCnpjCompetenciaAsync(orgId, cnpj, competencia);
                var sucesso = await _conciliacaoFolhaPontoRepository.CriarConciliacaoFolhaPontoAsync(conciliacaoFolhaPonto);

                if (!sucesso)
                {
                    throw new InvalidOperationException("Erro ao criar registro de conciliação de folha ponto");
                }

                foreach (var item in listaConciliacaoFolhaPontoColaborador)
                {
                    var itemLoteId = await _loteRepository.InserirItemLoteAsync(loteId, "");
                    item.ItemLoteId = itemLoteId;
                }
                
                
                _unitOfWork.Commit();

                foreach (var item in listaConciliacaoFolhaPontoColaborador)
                {
                    // Filtrar campos sensíveis do holerite
                    var holeriteFiltrado = FiltrarCamposHolerite(item.Holerite);
                    
                    // Filtrar campos sensíveis da folhaPonto
                    RelatorioPontoDTO? folhaPontoFiltrada = null;
                    if (item.FolhaPonto != null)
                    {
                        folhaPontoFiltrada = FiltrarCamposFolhaPonto(item.FolhaPonto.RelatorioPonto);
                    }

                    var bodySQS = JsonConvert.SerializeObject(
                        new
                        {
                            holerite = new { holerite = holeriteFiltrado },
                            folhaPonto = new { relatorioPonto = folhaPontoFiltrada },
                            item.Pagamentos,
                            item.Rubricas
                        },
                        new JsonSerializerSettings
                        {
                            ContractResolver =
                                new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                        }
                    );

                    await _queueProducer.SendMessageAsync(
                        queueUrl,
                        bodySQS, 
                        new Dictionary<string, string>
                    {
                        { "itemLoteId", item.ItemLoteId },
                        { "orgId", orgId.ToString() },
                        { "codigoInternoColaborador", item.CodigoInternoColaborador }
                    });
                }

                return sumario;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
            
        }
        

        public async Task<List<SumarioConciliacaoResult>> BuscarLotesPorOrgAsync(string codigoInternoColaborador, int orgId)
        {
            var lotes = await _conciliacaoFolhaPontoRepository.BuscarLotesPorOrgAsync(orgId);
            var lotesResult = new List<SumarioConciliacaoResult>();
            foreach (var lote in lotes)
            {
                var sumario = JsonConvert.DeserializeObject<SumarioConciliacaoResult>(lote.SumarioConciliacaoString);
                sumario.Id = lote.Id;
                sumario.QuantidadeItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(lote.Id);
                sumario.QuantidadeItensProcessadosComErro = await _loteRepository.GetTotalItensProcessadosComErroLoteAsync(lote.Id);

                var status = "Em processamento";
                sumario.StatusCode = StatusConciliacaoEnum.EM_PROCESSAMENTO;
                sumario.StatusConciliacao = "-";
                var conciliacao = await _conciliacaoFolhaPontoRepository.BuscarConciliacaoFolhaPontoPorLoteAsync(lote.Id);      
                if (conciliacao != null && conciliacao.Aprovado)
                {
                    // Conciliação aprovada
                    status = "Aprovado";
                    sumario.StatusConciliacao = "Aprovado";
                    sumario.StatusCode = StatusConciliacaoEnum.APROVADO;
                }               
                else if(sumario.QuantidadeItensProcessados == sumario.QuantidadeItens )
                {
                    var contemDivergencias = await _conciliacaoFolhaPontoRepository.GetLoteContemDivergenciasAsync(lote.Id);
                    sumario.StatusConciliacao = contemDivergencias ? "Processado com divergências" : "Processado com sucesso";
                    status = "Processado";
                    sumario.StatusCode = contemDivergencias ? StatusConciliacaoEnum.PROCESSADO_COM_DIVERGENCIA : StatusConciliacaoEnum.PROCESSADO_SEM_DIVERGENCIA;
                }
                sumario.Status = status;
                lotesResult.Add(sumario);
            }
            return lotesResult;
        }

        public async Task<ConciliacaoLoteResult> BuscarItensConciliacaoColaboradorAsync(string loteId, int orgId)
        {
            var lotes = await _conciliacaoFolhaPontoRepository.BuscarLotesPorOrgAsync(orgId);
            if(lotes.Count == 0 || !lotes.Any(x => x.Id == loteId))
            {
                throw new InvalidOperationException($"Lote com ID {loteId} não encontrado");
            }
            var conciliacaoLoteResult = new ConciliacaoLoteResult();
            conciliacaoLoteResult.ItensConciliacao = new List<ItemConciliacaoColaboradorDTO>();
            var sumarioLote = JsonConvert.DeserializeObject<SumarioConciliacaoResult>(lotes.Where(x => x.Id == loteId).First().SumarioConciliacaoString);
            conciliacaoLoteResult.SumarioConciliacao = sumarioLote;
            conciliacaoLoteResult.SumarioConciliacao.QuantidadeItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId);
            conciliacaoLoteResult.SumarioConciliacao.QuantidadeItensProcessadosComErro = await _loteRepository.GetTotalItensProcessadosComErroLoteAsync(loteId);
            
            var status = "Em processamento";
            conciliacaoLoteResult.SumarioConciliacao.StatusCode = StatusConciliacaoEnum.EM_PROCESSAMENTO;
            conciliacaoLoteResult.SumarioConciliacao.StatusConciliacao = "-";
            var conciliacao = await _conciliacaoFolhaPontoRepository.BuscarConciliacaoFolhaPontoPorLoteAsync(loteId);      
            if (conciliacao != null && conciliacao.Aprovado)
            {
                // Conciliação aprovada
                status = "Aprovado";
                conciliacaoLoteResult.SumarioConciliacao.StatusCode = StatusConciliacaoEnum.APROVADO;
            }      
            else if(conciliacaoLoteResult.SumarioConciliacao.QuantidadeItensProcessados == conciliacaoLoteResult.SumarioConciliacao.QuantidadeItens)
            {
                var contemDivergencias = await _conciliacaoFolhaPontoRepository.GetLoteContemDivergenciasAsync(loteId);
                conciliacaoLoteResult.SumarioConciliacao.StatusConciliacao = contemDivergencias ? "Processado com divergências" : "Processado com sucesso";
                status = "Processado";
                conciliacaoLoteResult.SumarioConciliacao.StatusCode = contemDivergencias ? StatusConciliacaoEnum.PROCESSADO_COM_DIVERGENCIA : StatusConciliacaoEnum.PROCESSADO_SEM_DIVERGENCIA;
            }
            conciliacaoLoteResult.SumarioConciliacao.Status = status;
            
            var itensConciliacao = await _conciliacaoFolhaPontoRepository.BuscarItensConciliacaoColaboradorAsync(loteId);
            var itensLote = await _loteRepository.ListarItensLoteAsync(loteId);
            foreach (var item in itensLote)
            {
                if(itensConciliacao.Any(x => x.ResultadoConciliacao.TbItemLoteId == item.Id))
                {
                    var itemConciliacao = itensConciliacao.Where(x => x.ResultadoConciliacao.TbItemLoteId == item.Id).First();
                    conciliacaoLoteResult.ItensConciliacao.Add(itemConciliacao);
                }
                else
                {
                    conciliacaoLoteResult.ItensConciliacao.Add(new ItemConciliacaoColaboradorDTO
                    {
                        NomeColaborador = "-",
                        CodigoInternoColaborador = "-",
                        Cargo = "-",
                        HoleritePath = null,
                        FolhaPontoPath = null,
                        ResultadoConciliacao = null
                    });
                }
            }
            //conciliacaoLoteResult.SumarioConciliacao.QuantidadeItensProcessadosComErro = conciliacaoLoteResult.ItensConciliacao.Sum(x => x.ResultadoConciliacao.NumeroDivergencias);
            return conciliacaoLoteResult;
        }

        public async Task<ApiGenericResult<FileContentResult>> ExportarDivergenciasLoteAsync(string loteId, int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                // Buscar itens de conciliação do lote
                var conciliacaoLoteResult = await BuscarItensConciliacaoColaboradorAsync(loteId, orgId);
                
                if (conciliacaoLoteResult?.ItensConciliacao == null || !conciliacaoLoteResult.ItensConciliacao.Any())
                {
                    ret.Mensagem = "Não existem itens de conciliação para o lote informado.";
                    ret.Sucesso = false;
                    return ret;
                }

                // Filtrar apenas itens que possuem divergências
                var itensComDivergencias = conciliacaoLoteResult.ItensConciliacao
                    .Where(x => x.ResultadoConciliacao != null && x.ResultadoConciliacao.HouveDivergencia)
                    .ToList();

                if (!itensComDivergencias.Any())
                {
                    ret.Mensagem = "Não existem divergências para exportar no lote informado.";
                    ret.Sucesso = false;
                    return ret;
                }

                // Criar lista de dados para exportação
                var dadosExportacao = new List<dynamic>();

                foreach (var item in itensComDivergencias)
                {
                    // Usar as divergências que já foram carregadas no item
                    if (item.ResultadoConciliacao.Divergencias != null && item.ResultadoConciliacao.Divergencias.Any())
                    {
                        foreach (var divergencia in item.ResultadoConciliacao.Divergencias)
                        {
                            var registro = new Dictionary<string, object>
                            {
                                { "NomeColaborador", item.NomeColaborador },
                                { "Cargo", item.Cargo },
                                { "CampoDivergencia", divergencia.CampoDivergencia },
                                { "Mensagem", divergencia.Mensagem },
                                { "ValorContabilidade", divergencia.ValorContabilidade },
                                { "DataProcessamento", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                            };
                            dadosExportacao.Add(registro);
                        }
                    }
                    else
                    {
                        // Se não há divergências específicas, adicionar registro geral
                        var registro = new Dictionary<string, object>
                        {
                            { "NomeColaborador", item.NomeColaborador },
                            { "Cargo", item.Cargo },
                            { "CampoDivergencia", "Geral" },
                            { "Mensagem", $"Houve {item.ResultadoConciliacao.NumeroDivergencias} divergência(s) identificada(s)" },
                            { "ValorContabilidade", "-" },
                            { "DataProcessamento", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                        };
                        dadosExportacao.Add(registro);
                    }
                }

                if (!dadosExportacao.Any())
                {
                    ret.Mensagem = "Não existem dados de divergências para exportar.";
                    ret.Sucesso = false;
                    return ret;
                }

                // Criar arquivo Excel
                var fileBytes = ExcelFileUtil.CreateExcelFile(dadosExportacao, "Divergências");
                var fileName = $"Divergencias_Lote_{loteId}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        private bool ValidarFormatoCompetencia(string competencia)
        {
            if (string.IsNullOrWhiteSpace(competencia) || competencia.Length != 7)
                return false;

            if (competencia[2] != '/')
                return false;

            if (!int.TryParse(competencia.Substring(0, 2), out int mes) || mes < 1 || mes > 12)
                return false;

            if (!int.TryParse(competencia.Substring(3, 4), out int ano) || ano < 1900 || ano > 2100)
                return false;

            return true;
        }

        public async Task ProcessarItemConciliacaoAsync(RetornoAnaliseHoleriteDTO retornoAnaliseHoleriteDTO, int orgId, string itemLoteId, string codigoInternoColaborador)
        {
            _unitOfWork.BeginTransaction();
            try
            {  
                var conciliacaoFolhaPonto = await _conciliacaoFolhaPontoRepository.BuscarConciliacaoFolhaPontoPorItemLoteAsync(itemLoteId);
                if (conciliacaoFolhaPonto == null)
                {
                    throw new ValidationException($"Conciliação folha ponto não encontrada para o item do lote {itemLoteId}");
                }
                try
                {   

                    // Calcular se houve divergência e número de divergências
                    var houveDivergencia = retornoAnaliseHoleriteDTO.HouveDivergencia;
                    var numeroDivergencias = retornoAnaliseHoleriteDTO.NumeroDeDivergencias;

                    // Criar o registro de conciliação do colaborador
                    var conciliacaoColaborador = new ConciliacaoFolhaPontoColaboradorDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        HouveDivergencia = houveDivergencia,
                        NumeroDivergencias = numeroDivergencias,
                        TbItemLoteId = itemLoteId,
                        TbConciliacaoFolhaPontoId = conciliacaoFolhaPonto.Id,
                        CodigoInternoColaborador = codigoInternoColaborador
                    };

                    await _conciliacaoFolhaPontoRepository.CriarConciliacaoFolhaPontoColaboradorAsync(conciliacaoColaborador);


                    foreach (var divergencia in retornoAnaliseHoleriteDTO.RelatorioDivergencias)
                    {

                        var divergenciaDTO = new ConciliacaoFolhaPontoDivergenciaDTO
                        {
                            Id = Guid.NewGuid().ToString(),
                            CampoDivergencia = divergencia.Campo,
                            Mensagem = divergencia.Mensagem,
                            ValorEsperado = divergencia.Divergencia?.ValorFourmakers ?? null,
                            ValorContabilidade = divergencia.Divergencia?.ValorContabilidade ?? null,
                            TbConciliacaoFolhaPontoColaboradorId = conciliacaoColaborador.Id,
                            Status = divergencia.Status,
                            Regra = divergencia.Regra,
                            Formula = divergencia.CalculoDetalhado?.Formula ?? null,
                            Passos = divergencia.CalculoDetalhado?.Passos ?? null,
                            Variaveis = divergencia.CalculoDetalhado?.Variaveis ?? null,
                            EhDivergencia = true
                        };

                        await _conciliacaoFolhaPontoRepository.CriarConciliacaoFolhaPontoDivergenciaAsync(divergenciaDTO);

                    }

                    foreach (var verificados in retornoAnaliseHoleriteDTO.RelatorioVerificados)
                    {

                        var divergenciaDTO = new ConciliacaoFolhaPontoDivergenciaDTO
                        {
                            Id = Guid.NewGuid().ToString(),
                            CampoDivergencia = verificados.Campo,
                            Mensagem = verificados.Mensagem,
                            ValorEsperado = verificados.Divergencia?.ValorFourmakers,
                            ValorContabilidade = verificados.Divergencia?.ValorContabilidade,
                            TbConciliacaoFolhaPontoColaboradorId = conciliacaoColaborador.Id,
                            Status = verificados.Status,
                            Regra = verificados.Regra,
                            Formula = verificados.CalculoDetalhado?.Formula ?? null,
                            Passos = verificados.CalculoDetalhado?.Passos ?? null,
                            Variaveis = verificados.CalculoDetalhado?.Variaveis ?? null,
                            EhDivergencia = false
                        };

                        await _conciliacaoFolhaPontoRepository.CriarConciliacaoFolhaPontoDivergenciaAsync(divergenciaDTO);

                    }

                    foreach (var informativos in retornoAnaliseHoleriteDTO.RelatorioInformativos)
                    {

                        var divergenciaDTO = new ConciliacaoFolhaPontoDivergenciaDTO
                        {
                            Id = Guid.NewGuid().ToString(),
                            CampoDivergencia = informativos.Campo,
                            Mensagem = informativos.Mensagem,
                            ValorEsperado = informativos.Divergencia?.ValorFourmakers,
                            ValorContabilidade = informativos.Divergencia?.ValorContabilidade,
                            TbConciliacaoFolhaPontoColaboradorId = conciliacaoColaborador.Id,
                            Status = informativos.Status,
                            Regra = informativos.Regra,
                            Formula = informativos.CalculoDetalhado?.Formula ?? null,
                            Passos = informativos.CalculoDetalhado?.Passos ?? null,
                            Variaveis = informativos.CalculoDetalhado?.Variaveis ?? null,
                            EhDivergencia = false
                        };

                        await _conciliacaoFolhaPontoRepository.CriarConciliacaoFolhaPontoDivergenciaAsync(divergenciaDTO);
                    }

                    // Atualizar o item do lote com sucesso
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, true, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemConciliacaoDTO{
                        Erros = new List<string> { },
                        StackTrace = "",
                        ProcessadoComSucesso = true,
                        RetornoAnaliseHolerite = retornoAnaliseHoleriteDTO
                    }), DateTime.Now);
                    
                    await FinalizarLoteAsync(conciliacaoFolhaPonto.TbLoteId);
                    
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    // Atualizar o item do lote com erro
                    await _loteRepository.AtualizarItemLoteAsync(itemLoteId, false, JsonConvert.SerializeObject(new RetornoProcessamentoMensagemConciliacaoDTO{
                        Erros = new List<string> { ex.Message },
                        StackTrace = ex.StackTrace,
                        ProcessadoComSucesso = false,
                        RetornoAnaliseHolerite = null
                    }), DateTime.Now);
                    
                    await FinalizarLoteAsync(conciliacaoFolhaPonto.TbLoteId);
                    _unitOfWork.Commit();
                }
                
            }
            catch (Exception ex)
            {
                _unitOfWork.SafeRollback();
                throw;
            }
        }

        private async Task FinalizarLoteAsync(string loteId)
        {
            var totalItensLote = await _loteRepository.GetTotalItensLoteAsync(loteId);
            var totalItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId);
            if(totalItensProcessados == totalItensLote)
            {
                await _loteRepository.AtualizarLoteAsync(loteId, true, DateTime.Now);
            }
        }

        private HoleriteDTO FiltrarCamposHolerite(HoleriteDTO holerite)
        {
            if (holerite == null) return null;

            // Remover campos sensíveis da empresa
            if (holerite.Empresa != null)
            {
                holerite.Empresa.Nome = null;
                holerite.Empresa.Localidade = null;
                holerite.Empresa.Cnpj = null;
            }

            // Remover campos sensíveis do funcionário
            if (holerite.Funcionario != null)
            {
                holerite.Funcionario.Nome = null;
                holerite.Funcionario.Cpf = null;
                holerite.Funcionario.Cbo = null;
                holerite.Funcionario.DataAdmissao = null;
            }

            holerite.InformacoesBancarias = null;

            // if(holerite.BasesDeCalculo != null)
            // {
            //     holerite.BasesDeCalculo.Irrf = null;
            //     holerite.BasesDeCalculo.FaixaIrrf = null;
            // }

            return holerite;
        }

        private RelatorioPontoDTO FiltrarCamposFolhaPonto(RelatorioPontoDTO relatorioPonto)
        {
            if (relatorioPonto == null) return null;

            // Remover campos sensíveis da empresa
            if (relatorioPonto.Empresa != null)
            {
                relatorioPonto.Empresa.CnpjCpf = null;
                relatorioPonto.Empresa.Endereco = null;
                relatorioPonto.Empresa.Nome = null;
            }

            // Remover campos sensíveis do funcionário
            if (relatorioPonto.Funcionario != null)
            {
                relatorioPonto.Funcionario.CarteiraDeTrabalho = null;
                relatorioPonto.Funcionario.Cpf = null;
                relatorioPonto.Funcionario.Nome = null;
                relatorioPonto.Funcionario.Pis = null;
            }

            return relatorioPonto;
        }

        public async Task<List<StatusVigenciaConciliacaoDTO>> ListarStatusVigenciaConciliacaoAsync(int orgId)
        {
            try
            {
                var resultado = new List<StatusVigenciaConciliacaoDTO>();
                
                // Buscar todos os lotes de folha ponto finalizados da organização
                var lotesFolhaPonto = await _folhaPontoRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.FOLHAPONTO);
                var lotesFolhaPontoFinalizados = lotesFolhaPonto.Where(l => l.DataFinalizacao.HasValue).ToList();
                
                // Buscar todos os lotes de holerite finalizados da organização
                var lotesHolerite = await _holeriteRepository.BuscarLotesPorOrgAsync(orgId, TipoFilaEnum.HOLERITE);
                var lotesHoleriteFinalizados = lotesHolerite.Where(l => l.DataFinalizacao.HasValue).ToList();
                
                // Criar um dicionário para agrupar por CNPJ e competência que possuem ambos folha ponto e holerite
                var combinacoesValidas = new Dictionary<string, (string cnpj, string competencia)>();
                
                // Processar lotes de folha ponto
                foreach (var lote in lotesFolhaPontoFinalizados)
                {
                    if (!string.IsNullOrEmpty(lote.SumarioFolhaPontoString))
                    {
                        var sumario = JsonConvert.DeserializeObject<SumarioFolhaPontoDTO>(lote.SumarioFolhaPontoString);
                        var chave = $"{sumario.Cnpj}_{sumario.Competencia}";
                        combinacoesValidas[chave] = (sumario.Cnpj, sumario.Competencia);
                    }
                }
                
                // Verificar quais combinações também possuem holerite
                var combinacoesCompletas = new List<(string cnpj, string competencia)>();
                foreach (var lote in lotesHoleriteFinalizados)
                {
                    if (!string.IsNullOrEmpty(lote.SumarioHoleriteString))
                    {
                        var sumario = JsonConvert.DeserializeObject<SumarioHoleriteDTO>(lote.SumarioHoleriteString);
                        if(sumario?.Adiantamento != null && sumario.Adiantamento == true)
                        {
                            continue;
                        }
                        
                        if(sumario?.Ferias != null && sumario.Ferias == true)
                        {
                            continue;
                        }
                        
                        if(sumario?.DecimoTerceiro != null && sumario.DecimoTerceiro == true)
                        {
                            continue;
                        }
                        
                        if(sumario?.DecimoTerceiroAdiantamento != null && sumario.DecimoTerceiroAdiantamento == true)
                        {
                            continue;
                        }
                        var chave = $"{sumario.Cnpj}_{sumario.Competencia}";
                        
                        if (combinacoesValidas.ContainsKey(chave) && !combinacoesCompletas.Any(x => x.cnpj == sumario.Cnpj && x.competencia == sumario.Competencia))
                        {
                            combinacoesCompletas.Add((sumario.Cnpj, sumario.Competencia));
                        }
                    }
                }
                
                // Buscar todas as conciliações da organização
                var lotesConciliacao = await _conciliacaoFolhaPontoRepository.BuscarLotesPorOrgAsync(orgId);
                
                // Processar cada combinação válida (CNPJ + Vigência com folha ponto e holerite)
                foreach (var (cnpj, competencia) in combinacoesCompletas)
                {
                    var statusConciliacao = StatusConciliacaoEnum.NAO_PROCESSADO;
                    
                    // Buscar conciliações para este CNPJ e competência
                    var conciliacoesParaCnpjCompetencia = lotesConciliacao
                        .Where(l => !string.IsNullOrEmpty(l.SumarioConciliacaoString))
                        .Select(l => new { 
                            Lote = l, 
                            Sumario = JsonConvert.DeserializeObject<SumarioConciliacaoResult>(l.SumarioConciliacaoString) 
                        })
                        .Where(x => x.Sumario.Cnpj == cnpj && x.Sumario.Competencia == competencia)
                        .OrderByDescending(x => x.Lote.DataCriacao)
                        .ToList();
                    
                    if (conciliacoesParaCnpjCompetencia.Any())
                    {
                        var conciliacaoMaisRecente = conciliacoesParaCnpjCompetencia.First();
                        
                        // Buscar dados da conciliação para verificar se foi aprovada
                        var conciliacao = await _conciliacaoFolhaPontoRepository.BuscarConciliacaoFolhaPontoPorLoteAsync(conciliacaoMaisRecente.Lote.Id);
                        
                        if (conciliacao != null && conciliacao.Aprovado)
                        {
                            // Conciliação aprovada
                            statusConciliacao = StatusConciliacaoEnum.APROVADO;
                        }
                        else
                        {
                            // Verificar se está em processamento
                            var totalItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(conciliacaoMaisRecente.Lote.Id);
                            var totalItens = conciliacaoMaisRecente.Sumario.QuantidadeItens;
                            
                            if (totalItensProcessados < totalItens)
                            {
                                // Ainda em processamento
                                statusConciliacao = StatusConciliacaoEnum.EM_PROCESSAMENTO;
                            }
                            else
                            {
                                // Processamento concluído - verificar se tem divergências
                                var contemDivergencias = await _conciliacaoFolhaPontoRepository.GetLoteContemDivergenciasAsync(conciliacaoMaisRecente.Lote.Id);
                                statusConciliacao = contemDivergencias ? 
                                    StatusConciliacaoEnum.PROCESSADO_COM_DIVERGENCIA : 
                                    StatusConciliacaoEnum.PROCESSADO_SEM_DIVERGENCIA;
                            }
                        }
                    }
                    
                    resultado.Add(new StatusVigenciaConciliacaoDTO
                    {
                        Cnpj = cnpj,
                        Vigencia = competencia,
                        Status = statusConciliacao,
                        Descricao = await _projetoOrgRepository.ObterDescricaoProjeto(cnpj, orgId)
                    });
                }
                
                // Ordenar por CNPJ e vigência
                return resultado.OrderBy(s => s.Cnpj).ThenBy(s => s.Vigencia).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar status de vigência da conciliação: {ex.Message}", ex);
            }
        }

        public async Task<bool> AprovarConciliacaoAsync(int orgId, string loteId)
        {
            try
            {
                // Buscar o lote de conciliação
                var lotes = await _conciliacaoFolhaPontoRepository.BuscarLotesPorOrgAsync(orgId);
                var lote = lotes.FirstOrDefault(l => l.Id == loteId);
                
                if (lote == null)
                {
                    throw new InvalidOperationException($"Lote com ID {loteId} não encontrado na organização {orgId}");
                }

                // Verificar se o lote está processado (concluído)
                var totalItensProcessados = await _loteRepository.GetTotalItensProcessadosLoteAsync(loteId);
                var sumario = JsonConvert.DeserializeObject<SumarioConciliacaoResult>(lote.SumarioConciliacaoString);
                
                if (totalItensProcessados < sumario.QuantidadeItens)
                {
                    throw new InvalidOperationException($"Lote {loteId} ainda não foi totalmente processado. Processados: {totalItensProcessados}/{sumario.QuantidadeItens}");
                }

                // Verificar se já foi aprovado
                var conciliacao = await _conciliacaoFolhaPontoRepository.BuscarConciliacaoFolhaPontoPorLoteAsync(loteId);
                if (conciliacao == null)
                {
                    throw new InvalidOperationException($"Conciliação não encontrada para o lote {loteId}");
                }

                if (conciliacao.Aprovado)
                {
                    throw new InvalidOperationException($"Lote {loteId} já foi aprovado anteriormente");
                }
                
                await _conciliacaoFolhaPontoRepository.DesaprovarConciliacaoPorCnpjCompetenciaAsync(orgId, sumario.Cnpj, sumario.Competencia);
                // Aprovar a conciliação
                var sucesso = await _conciliacaoFolhaPontoRepository.AtualizarStatusAprovacaoAsync(orgId, loteId, true);
                
                if (!sucesso)
                {
                    throw new InvalidOperationException($"Erro ao aprovar conciliação do lote {loteId}");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao aprovar conciliação: {ex.Message}", ex);
            }
        }

    }
} 