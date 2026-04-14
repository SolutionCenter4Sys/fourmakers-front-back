using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Gestor;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Impl.GestaoDesempenho.Gestor
{
    public class GestaoDesempenhoGestorService : IGestaoDesempenhoGestorService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Gestão de Desempenho";
        private const int FREQUENCIA_PADRAO_ONE_ON_ONE_DIAS = 14;
        private const int FREQUENCIA_PADRAO_FEEDBACK_DIAS = 30;

        private readonly IGestaoDesempenhoGestorRepository _gestorRepository;
        private readonly IGestaoDesempenhoParametrizacaoRepository _parametrizacaoRepository;
        private readonly IGestaoDesempenhoColaboradorRepository _colaboradorRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public GestaoDesempenhoGestorService(
            IGestaoDesempenhoGestorRepository gestorRepository,
            IGestaoDesempenhoParametrizacaoRepository parametrizacaoRepository,
            IGestaoDesempenhoColaboradorRepository colaboradorRepository,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _gestorRepository = gestorRepository;
            _parametrizacaoRepository = parametrizacaoRepository;
            _colaboradorRepository = colaboradorRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<ApiGenericResult<DashboardGestorDTO>> ObterDashboardGestorAsync(string codigoInternoColaboradorGestor, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DashboardGestorDTO>();

            try
            {
                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradoresSubordinados = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                if (colaboradoresSubordinados == null || !colaboradoresSubordinados.Any())
                {
                    apiGenericResult.Retorno = new DashboardGestorDTO
                    {
                        PainelControle = new PainelControleDTO
                        {
                            QtdTotalColaboradores = 0,
                            QtdOneOnOneEmDia = 0,
                            QtdOneOnOneAtrasado = 0,
                            QtdFeedbackEmDia = 0,
                            QtdFeedbackAtrasado = 0
                        },
                        OneOnOnesComRegistroCritico = new System.Collections.Generic.List<OneOnOneRegistroCriticoDTO>()
                    };
                    return apiGenericResult;
                }

                var codigosInternosColaboradores = colaboradoresSubordinados
                    .Select(c => c.CodigoInternoColaborador)
                    .ToList();

                var ultimasDataOneOnOne = await _gestorRepository.ObterUltimasDataOneOnOneAsync(codigosInternosColaboradores);
                var ultimasDataFeedback = await _gestorRepository.ObterUltimasDataFeedbackAsync(codigosInternosColaboradores);
                var parametrizacao = await _parametrizacaoRepository.ObterParametrizacaoPorOrgAsync(orgId);
                var registrosCriticos = await _gestorRepository.ObterRegistrosCriticosAsync(codExternoGestor, orgId);

                int qtdTotal = colaboradoresSubordinados.Count;
                int qtdOneOnOneEmDia = 0;
                int qtdOneOnOneAtrasado = 0;
                int qtdFeedbackEmDia = 0;
                int qtdFeedbackAtrasado = 0;

                int frequenciaOneOnOneDias = parametrizacao?.FrequenciaEsperadaOneOnOneDias ?? FREQUENCIA_PADRAO_ONE_ON_ONE_DIAS;
                int frequenciaFeedbackDias = parametrizacao?.FrequenciaEsperadaFeedbackDias ?? FREQUENCIA_PADRAO_FEEDBACK_DIAS;

                foreach (var colab in colaboradoresSubordinados)
                {
                    ultimasDataOneOnOne.TryGetValue(colab.CodigoInternoColaborador, out var ultimaDataOneOnOne);
                    ultimasDataFeedback.TryGetValue(colab.CodigoInternoColaborador, out var ultimaDataFeedback);

                    // Análise OneOnOne
                    if (!ultimaDataOneOnOne.HasValue)
                    {
                        qtdOneOnOneAtrasado++;
                    }
                    else
                    {
                        var diasDesdeUltimo = (DateTime.Now - ultimaDataOneOnOne.Value).Days;
                        if (diasDesdeUltimo <= frequenciaOneOnOneDias)
                        {
                            qtdOneOnOneEmDia++;
                        }
                        else
                        {
                            qtdOneOnOneAtrasado++;
                        }
                    }

                    // Análise Feedback
                    if (!ultimaDataFeedback.HasValue)
                    {
                        qtdFeedbackAtrasado++;
                    }
                    else
                    {
                        var diasDesdeUltimo = (DateTime.Now - ultimaDataFeedback.Value).Days;
                        if (diasDesdeUltimo <= frequenciaFeedbackDias)
                        {
                            qtdFeedbackEmDia++;
                        }
                        else
                        {
                            qtdFeedbackAtrasado++;
                        }
                    }
                }

                apiGenericResult.Retorno = new DashboardGestorDTO
                {
                    PainelControle = new PainelControleDTO
                    {
                        QtdTotalColaboradores = qtdTotal,
                        QtdOneOnOneEmDia = qtdOneOnOneEmDia,
                        QtdOneOnOneAtrasado = qtdOneOnOneAtrasado,
                        QtdFeedbackEmDia = qtdFeedbackEmDia,
                        QtdFeedbackAtrasado = qtdFeedbackAtrasado
                    },
                    OneOnOnesComRegistroCritico = registrosCriticos
                };
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<MeusColaboradoresResponseDTO>> ObterMeusColaboradoresAsync(string codigoInternoColaboradorGestor, int orgId, MeusColaboradoresRequestDTO filtros)
        {
            var apiGenericResult = new ApiGenericResult<MeusColaboradoresResponseDTO>();

            try
            {
                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradores = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                if (colaboradores == null || !colaboradores.Any())
                {
                    apiGenericResult.Retorno = new MeusColaboradoresResponseDTO
                    {
                        MeusColaboradores = new System.Collections.Generic.List<ColaboradorDetalhesDTO>()
                    };
                    return apiGenericResult;
                }

                var codigosInternos = colaboradores.Select(c => c.CodigoInternoColaborador).ToList();

                var ultimasDataOneOnOne = await _gestorRepository.ObterUltimasDataOneOnOneAsync(codigosInternos);
                var ultimasDataFeedback = await _gestorRepository.ObterUltimasDataFeedbackAsync(codigosInternos);
                var parametrizacao = await _parametrizacaoRepository.ObterParametrizacaoPorOrgAsync(orgId);

                foreach (var colab in colaboradores)
                {
                    ultimasDataOneOnOne.TryGetValue(colab.CodigoInternoColaborador, out var ultimaDataOneOnOne);
                    ultimasDataFeedback.TryGetValue(colab.CodigoInternoColaborador, out var ultimaDataFeedback);

                    colab.DataUltimoOneOnOne = ultimaDataOneOnOne;
                    colab.DataUltimoFeedback = ultimaDataFeedback;
                }

                int frequenciaOneOnOneDias = parametrizacao?.FrequenciaEsperadaOneOnOneDias ?? FREQUENCIA_PADRAO_ONE_ON_ONE_DIAS;
                int frequenciaFeedbackDias = parametrizacao?.FrequenciaEsperadaFeedbackDias ?? FREQUENCIA_PADRAO_FEEDBACK_DIAS;

                var colaboradoresFiltrados = colaboradores.AsEnumerable();

                if (filtros?.SemFeedback == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c => !c.DataUltimoFeedback.HasValue);
                }

                if (filtros?.SemOneOnOne == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c => !c.DataUltimoOneOnOne.HasValue);
                }

                if (filtros?.SemOneOnOneAcimaDeParametroDias == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c =>
                        !c.DataUltimoOneOnOne.HasValue ||
                        (DateTime.Now - c.DataUltimoOneOnOne.Value).Days > frequenciaOneOnOneDias
                    );
                }

                if (filtros?.SemFeedbackAcimaDeParametroDias == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c =>
                        !c.DataUltimoFeedback.HasValue ||
                        (DateTime.Now - c.DataUltimoFeedback.Value).Days > frequenciaFeedbackDias
                    );
                }

                apiGenericResult.Retorno = new MeusColaboradoresResponseDTO
                {
                    MeusColaboradores = colaboradoresFiltrados.ToList()
                };
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task<string> ObterCodigoExternoGestorAsync(string codigoInternoColaboradorGestor, int orgId)
        {
            var codExternoGestor = await _gestorRepository.ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);

            if (string.IsNullOrEmpty(codExternoGestor))
            {
                throw new ApplicationException($"Gestor com código interno {codigoInternoColaboradorGestor} não encontrado ou inativo na organização {orgId}");
            }

            return codExternoGestor;
        }

        private async Task<List<ColaboradorDetalhesDTO>> ListarMeusColaboradoresSubordinados(string codExternoGestor, int orgId, bool recursivo = true)
        {

            var colaboradores = await _gestorRepository.ObterColaboradoresSubordinadosComDetalhesAsync(codExternoGestor, orgId);

            if (!recursivo)
            {
                return colaboradores;
            }

            List<ColaboradorDetalhesDTO> colaboradoresResult = new();
            HashSet<string> visitados = new HashSet<string>();
            await BuscarColaboradoresSubordinadosRecursivo(orgId, colaboradores, colaboradoresResult, visitados);
            return colaboradoresResult;

        }

        private async Task BuscarColaboradoresSubordinadosRecursivo(
            int orgId,
            List<ColaboradorDetalhesDTO> colaboradores,
            List<ColaboradorDetalhesDTO> resultado,
            HashSet<string> visitados)
        {
            foreach (var colaborador in colaboradores)
            {
                // Se já foi processado, pula
                if (!visitados.Add(colaborador.CodColaboradorExterno))
                    continue;
 
                resultado.Add(colaborador);

                var subordinados =
                        await _gestorRepository.ObterColaboradoresSubordinadosComDetalhesAsync(
                            colaborador.CodColaboradorExterno, orgId);

                if (subordinados != null && subordinados.Any())
                {
                    await BuscarColaboradoresSubordinadosRecursivo(
                            orgId,
                            subordinados,
                    resultado,
                    visitados);
                }
            }
        }

        public async Task<ApiGenericResult<InserirFeedbackRequestDTO>> InserirFeedbackAsync(string codigoInternoColaboradorGestor, int orgId, InserirFeedbackRequestDTO request)
        {
            var apiGenericResult = new ApiGenericResult<InserirFeedbackRequestDTO>();

            try
            {
                if (request == null)
                {
                    throw new ArgumentException(nameof(request), "Request não pode ser nulo");
                }

                if (string.IsNullOrEmpty(request.CodigoInternoColaboradorAvaliado))
                {
                    throw new ArgumentException("Código interno do colaborador avaliado é obrigatório", nameof(request.CodigoInternoColaboradorAvaliado));
                }

                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradoresSubordinados = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                ValidarColaboradorSubordinadoOuRH(colaboradoresSubordinados, request.CodigoInternoColaboradorAvaliado, codigoInternoColaboradorGestor, orgId);

                var feedbackId = string.IsNullOrEmpty(request.Id) ? Guid.NewGuid().ToString() : request.Id;
                request.Id = feedbackId;

                await _gestorRepository.InserirFeedbackAsync(feedbackId, codigoInternoColaboradorGestor, request);

                apiGenericResult.Retorno = request;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<InserirOneOnOneRequestDTO>> InserirOneOnOneAsync(string codigoInternoColaboradorGestor, int orgId, InserirOneOnOneRequestDTO request)
        {
            var apiGenericResult = new ApiGenericResult<InserirOneOnOneRequestDTO>();

            try
            {
                if (request == null)
                {
                    throw new ArgumentException(nameof(request), "Request não pode ser nulo");
                }

                if (string.IsNullOrEmpty(request.CodigoInternoColaboradorAvaliado))
                {
                    throw new ArgumentException("Código interno do colaborador avaliado é obrigatório", nameof(request.CodigoInternoColaboradorAvaliado));
                }

                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradoresSubordinados = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                ValidarColaboradorSubordinadoOuRH(colaboradoresSubordinados, request.CodigoInternoColaboradorAvaliado, codigoInternoColaboradorGestor, orgId);

                var oneOnOneId = await _gestorRepository.InserirOneOnOneAsync(codigoInternoColaboradorGestor, request);

                apiGenericResult.Retorno = request;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<DashboardColaboradorDTO>> ObterDashboardColaboradorAsync(string codigoInternoColaboradorGestor, string codigoInternoColaboradorAvaliado, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DashboardColaboradorDTO>();

            try
            {
                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradoresSubordinados = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                ValidarColaboradorSubordinadoOuRH(colaboradoresSubordinados, codigoInternoColaboradorAvaliado, codigoInternoColaboradorGestor, orgId);

                var dashboard = await _gestorRepository.ObterDashboardColaboradorAsync(codigoInternoColaboradorAvaliado, orgId);

                if (dashboard == null)
                {
                    throw new ApplicationException($"Colaborador {codigoInternoColaboradorAvaliado} não encontrado");
                }

                var feedbacks = await _gestorRepository.ObterFeedbacksColaboradorAsync(codigoInternoColaboradorAvaliado);
                var oneOnOnes = await _gestorRepository.ObterOneOnOnesColaboradorAsync(codigoInternoColaboradorAvaliado);
                var pautasSugeridas = await _colaboradorRepository.ObterPautasSugeridasPorColaboradorAvalidadoAsync(codigoInternoColaboradorAvaliado);

                dashboard.DashboardFeedbacks = new DashboardFeedbacksDTO
                {
                    QtdVistos = feedbacks.Count(f => f.VisualizadoPeloColaborador == true),
                    QtdNaoVistos = feedbacks.Count(f => f.VisualizadoPeloColaborador == false || f.VisualizadoPeloColaborador == null),
                    Feedbacks = feedbacks
                };

                dashboard.DashboardOneOnOne = new DashboardOneOnOneDTO
                {
                    QtdVistos = oneOnOnes.Count(o => o.VisualizadoPeloColaborador == true),
                    QtdNaoVistos = oneOnOnes.Count(o => o.VisualizadoPeloColaborador == false || o.VisualizadoPeloColaborador == null),
                    PautasSugeridas = pautasSugeridas,
                    OneOnOnes = oneOnOnes
                };

                apiGenericResult.Retorno = dashboard;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<InserirPautaSugeridaGestorResponseDTO>> InserirPautaSugeridaGestorAsync(string codigoInternoColaboradorGestor, int orgId, InserirPautaSugeridaGestorRequestDTO request)
        {
            var apiGenericResult = new ApiGenericResult<InserirPautaSugeridaGestorResponseDTO>();

            try
            {
                if (request == null)
                {
                    throw new ArgumentException(nameof(request), "Request não pode ser nulo");
                }

                if (string.IsNullOrEmpty(request.CodigoInternoColaboradorAvaliado))
                {
                    throw new ArgumentException("Código interno do colaborador avaliado é obrigatório", nameof(request.CodigoInternoColaboradorAvaliado));
                }

                var codExternoGestor = await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                var colaboradoresSubordinados = await ListarMeusColaboradoresSubordinados(codExternoGestor, orgId);

                ValidarColaboradorSubordinadoOuRH(colaboradoresSubordinados, request.CodigoInternoColaboradorAvaliado, codigoInternoColaboradorGestor, orgId);

                var resultado = await _gestorRepository.UpsertPautaSugeridaGestorAsync(codigoInternoColaboradorGestor, request);

                apiGenericResult.Retorno = resultado;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }
        
        public async Task<ApiGenericResult> AtualizarRegistroCriticoOneOnOneAsync(string codigoInternoColaboradorGestor, int orgId, string oneOnOneId, bool registroCritico)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                await ObterCodigoExternoGestorAsync(codigoInternoColaboradorGestor, orgId);
                apiGenericResult.Sucesso = await _gestorRepository.AtualizarRegistroCriticoOneOnOneAsync(oneOnOneId, registroCritico);
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private void ValidarColaboradorSubordinadoOuRH(
            List<ColaboradorDetalhesDTO> colaboradoresSubordinados,
            string codigoInternoColaboradorAvaliado,
            string codigoInternoColaboradorGestor,
            int orgId)
        {
            var ehSubordinado = colaboradoresSubordinados != null &&
                colaboradoresSubordinados.Any(c => c.CodigoInternoColaborador == codigoInternoColaboradorAvaliado);

            if (!ehSubordinado && !ValidaAcessoGestaoDesempenhoRH(codigoInternoColaboradorGestor, orgId))
            {
                throw new ApplicationException($"Colaborador {codigoInternoColaboradorAvaliado} não é subordinado do gestor {codigoInternoColaboradorGestor}");
            }
        }

        private bool ValidaAcessoGestaoDesempenhoRH(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.GESTAO_DESEMPENHO_RH);

            return isValid.Result;
        }

    }
}
