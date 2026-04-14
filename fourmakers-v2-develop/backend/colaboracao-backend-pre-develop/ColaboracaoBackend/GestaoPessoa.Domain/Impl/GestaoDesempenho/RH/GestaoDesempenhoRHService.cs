using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using Core.Domain.GestaoPessoa.GestaoDesempenho.RH;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.RH;

namespace GestaoPessoa.Domain.Impl.GestaoDesempenho.RH
{
    public class GestaoDesempenhoRHService : IGestaoDesempenhoRHService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Gestão de Desempenho - RH";
        private const int FREQUENCIA_PADRAO_ONE_ON_ONE_DIAS = 14;
        private const int FREQUENCIA_PADRAO_FEEDBACK_DIAS = 30;

        private readonly IGestaoDesempenhoRHRepository _rhRepository;
        private readonly IGestaoDesempenhoParametrizacaoRepository _parametrizacaoRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public GestaoDesempenhoRHService(
            IGestaoDesempenhoRHRepository rhRepository,
            IGestaoDesempenhoParametrizacaoRepository parametrizacaoRepository,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _rhRepository = rhRepository;
            _parametrizacaoRepository = parametrizacaoRepository;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<ApiGenericResult<DashboardRHDTO>> ObterDashboardRHAsync(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<DashboardRHDTO>();

            try
            {

                ValidaAcessoGestaoDesempenhoRHCrud(cpfRequest, orgId);

                var qtdTotalColaboradores = await _rhRepository.ObterQtdTotalColaboradoresAsync(orgId);

                if (qtdTotalColaboradores == 0)
                {
                    apiGenericResult.Retorno = new DashboardRHDTO
                    {
                        QtdTotalColaboradores = 0,
                        PorcentagemGestoresOneOnOneEmDia = 0,
                        PorcentagemGestoresFeedbackEmDia = 0,
                        QtdSemOneOnOneHaMaisQtdParametroDias = 0,
                        QtdSemFeedbackHaMaisQtdParametroDias = 0
                    };
                    return apiGenericResult;
                }

                var codigosColaboradores = await _rhRepository.ObterTodosCodigosInternosColaboradoresAsync(orgId);
                var ultimasDataOneOnOne = await _rhRepository.ObterTodasUltimasDataOneOnOneAsync(orgId);
                var ultimasDataFeedback = await _rhRepository.ObterTodasUltimasDataFeedbackAsync(orgId);
                var parametrizacao = await _parametrizacaoRepository.ObterParametrizacaoPorOrgAsync(orgId);

                int qtdOneOnOneEmDia = 0;
                int qtdFeedbackEmDia = 0;
                int qtdSemOneOnOneHaMaisQtdParametroDias = 0;
                int qtdSemFeedbackHaMaisQtdParametroDias = 0;

                int frequenciaOneOnOneDias = parametrizacao?.FrequenciaEsperadaOneOnOneDias ?? FREQUENCIA_PADRAO_ONE_ON_ONE_DIAS;
                int frequenciaFeedbackDias = parametrizacao?.FrequenciaEsperadaFeedbackDias ?? FREQUENCIA_PADRAO_FEEDBACK_DIAS;

                foreach (var codigoColaborador in codigosColaboradores)
                {
                    ultimasDataOneOnOne.TryGetValue(codigoColaborador, out var ultimaDataOneOnOne);
                    ultimasDataFeedback.TryGetValue(codigoColaborador, out var ultimaDataFeedback);

                    // Análise OneOnOne
                    if (!ultimaDataOneOnOne.HasValue)
                    {
                        qtdSemOneOnOneHaMaisQtdParametroDias++;
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
                            qtdSemOneOnOneHaMaisQtdParametroDias++;
                        }
                    }

                    // Análise Feedback
                    if (!ultimaDataFeedback.HasValue)
                    {
                        qtdSemFeedbackHaMaisQtdParametroDias++;
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
                            qtdSemFeedbackHaMaisQtdParametroDias++;
                        }
                    }
                }

                // Calcular porcentagens
                decimal porcentagemOneOnOneEmDia = qtdTotalColaboradores > 0
                    ? Math.Round((decimal)qtdOneOnOneEmDia / qtdTotalColaboradores * 100, 2)
                    : 0;

                decimal porcentagemFeedbackEmDia = qtdTotalColaboradores > 0
                    ? Math.Round((decimal)qtdFeedbackEmDia / qtdTotalColaboradores * 100, 2)
                    : 0;

                apiGenericResult.Retorno = new DashboardRHDTO
                {
                    QtdTotalColaboradores = qtdTotalColaboradores,
                    PorcentagemGestoresOneOnOneEmDia = porcentagemOneOnOneEmDia,
                    PorcentagemGestoresFeedbackEmDia = porcentagemFeedbackEmDia,
                    QtdSemOneOnOneHaMaisQtdParametroDias = qtdSemOneOnOneHaMaisQtdParametroDias,
                    QtdSemFeedbackHaMaisQtdParametroDias = qtdSemFeedbackHaMaisQtdParametroDias
                };
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ListaColaboradoresResponseDTO>> ObterListaColaboradoresAsync(string cpfRequest, int orgId, ListaColaboradoresRequestDTO filtros)
        {
            var apiGenericResult = new ApiGenericResult<ListaColaboradoresResponseDTO>();

            try
            {
                ValidaAcessoGestaoDesempenhoRHCrud(cpfRequest, orgId);

                var colaboradores = await _rhRepository.ObterTodosColaboradoresComDetalhesAsync(orgId);

                if (colaboradores == null || !colaboradores.Any())
                {
                    apiGenericResult.Retorno = new ListaColaboradoresResponseDTO
                    {
                        Colaboradores = new List<ColaboradorRHDTO>()
                    };
                    return apiGenericResult;
                }

                var codigosInternos = colaboradores.Select(c => c.CodigoInternoColaborador).ToList();

                var ultimasDataOneOnOne = await _rhRepository.ObterTodasUltimasDataOneOnOneAsync(orgId);
                var ultimasDataFeedback = await _rhRepository.ObterTodasUltimasDataFeedbackAsync(orgId);
                var parametrizacao = await _parametrizacaoRepository.ObterParametrizacaoPorOrgAsync(orgId);

                // Popula as datas nos colaboradores
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

                // Aplicar filtros
                if (filtros?.SemFeedback == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c => !c.DataUltimoFeedback.HasValue);
                }

                if (filtros?.SemOneOnOne == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c => !c.DataUltimoOneOnOne.HasValue);
                }

                if (filtros?.SemOneOnOneDias == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c =>
                        !c.DataUltimoOneOnOne.HasValue ||
                        (DateTime.Now - c.DataUltimoOneOnOne.Value).Days > frequenciaOneOnOneDias
                    );
                }

                if (filtros?.SemFeedbackDias == true)
                {
                    colaboradoresFiltrados = colaboradoresFiltrados.Where(c =>
                        !c.DataUltimoFeedback.HasValue ||
                        (DateTime.Now - c.DataUltimoFeedback.Value).Days > frequenciaFeedbackDias
                    );
                }

                apiGenericResult.Retorno = new ListaColaboradoresResponseDTO
                {
                    Colaboradores = colaboradoresFiltrados.ToList()
                };
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<InserirParametrizacaoRequestDTO>> InserirParametrizacaoAsync(string cpfRequest, int orgId, InserirParametrizacaoRequestDTO request)
        {
            var apiGenericResult = new ApiGenericResult<InserirParametrizacaoRequestDTO>();

            try
            {
                ValidaAcessoGestaoDesempenhoRHCrud(cpfRequest, orgId);

                var parametrizacao = new ParametrizacaoOrgDTO
                {
                    OrgId = orgId,
                    FrequenciaEsperadaOneOnOneDias = request.FrequenciaEsperadaOneOnOneDias,
                    FrequenciaEsperadaFeedbackDias = request.FrequenciaEsperadaFeedbackDias
                };

                var sucesso = await _parametrizacaoRepository.UpsertParametrizacaoAsync(parametrizacao);

                if (sucesso)
                {
                    apiGenericResult.Retorno = request;
                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<ParametrizacaoOrgDTO?>> ObterParametrizacaoPorOrgAsync(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<ParametrizacaoOrgDTO>();

            try
            {
                ValidaAcessoGestaoDesempenhoRHCrud(cpfRequest, orgId);

                var parametrizacao = await _parametrizacaoRepository.ObterParametrizacaoPorOrgAsync(orgId);

                apiGenericResult.Retorno = parametrizacao;

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private void ValidaAcessoGestaoDesempenhoRHCrud(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.GESTAO_DESEMPENHO_RH);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para funcionalidade 'GESTAO_DESEMPENHO_RH'."); //não pode adicionar, editar e remover
            }
        }
    }
}
