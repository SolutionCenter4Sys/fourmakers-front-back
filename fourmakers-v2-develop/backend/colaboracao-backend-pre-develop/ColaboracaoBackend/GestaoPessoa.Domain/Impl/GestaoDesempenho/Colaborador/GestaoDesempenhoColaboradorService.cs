using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Colaborador;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Impl.GestaoDesempenho.Colaborador
{
    public class GestaoDesempenhoColaboradorService : IGestaoDesempenhoColaboradorService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Gestão de Desempenho - Colaborador";

        private readonly IGestaoDesempenhoColaboradorRepository _colaboradorRepository;

        public GestaoDesempenhoColaboradorService(IGestaoDesempenhoColaboradorRepository colaboradorRepository)
        {
            _colaboradorRepository = colaboradorRepository;
        }

        public async Task<ApiGenericResult<ColaboradorDashboardDTO>> ObterMeuDashboardAsync(string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<ColaboradorDashboardDTO>();

            try
            {
                var feedbacks = await _colaboradorRepository.ObterMeusFeedbacksAsync(codigoInternoColaborador);
                var oneOnOnes = await _colaboradorRepository.ObterMeusOneOnOnesAsync(codigoInternoColaborador);
                var pautasSugeridas = await _colaboradorRepository.ObterPautasSugeridasPorColaboradorAvalidadoAsync(codigoInternoColaborador);
                var registrosCriticos = await _colaboradorRepository.ObterMeusRegistrosCriticosAsync(codigoInternoColaborador);

                var dashboard = new ColaboradorDashboardDTO
                {
                    MeuPainel = new MeuPainelDTO
                    {
                        QtdFeedbacks = feedbacks.Count,
                        QtdOneOnOne = oneOnOnes.Count
                    },
                    RegistrosCriticos = registrosCriticos,
                    DashboardFeedbacks = new ColaboradorDashboardFeedbacksDTO
                    {
                        QtdVistos = feedbacks.Count(f => f.VisualizadoPeloColaborador == true),
                        QtdNaoVistos = feedbacks.Count(f => f.VisualizadoPeloColaborador == false || f.VisualizadoPeloColaborador == null),
                        Feedbacks = feedbacks
                    },
                    DashboardOneOnOne = new ColaboradorDashboardOneOnOneDTO
                    {
                        QtdVistos = oneOnOnes.Count(o => o.VisualizadoPeloColaborador == true),
                        QtdNaoVistos = oneOnOnes.Count(o => o.VisualizadoPeloColaborador == false || o.VisualizadoPeloColaborador == null),
                        PautasSugeridas = pautasSugeridas,
                        OneOnOnes = oneOnOnes
                    }
                };

                apiGenericResult.Retorno = dashboard;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<string>> InserirVisualizacaoFeedbackAsync(string feedbackId, string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<string>();

            try
            {
                if (string.IsNullOrEmpty(feedbackId))
                {
                    throw new ArgumentException("ID do feedback é obrigatório", nameof(feedbackId));
                }

                var sucesso = await _colaboradorRepository.InserirVisualizacaoFeedbackAsync(feedbackId, codigoInternoColaborador);

                if (!sucesso)
                {
                    throw new ApplicationException($"Feedback {feedbackId} não encontrado ou não pertence ao colaborador");
                }

                apiGenericResult.Retorno = "Visualização do feedback registrada com sucesso";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<string>> InserirVisualizacaoOneOnOneAsync(string oneOnOneId, string codigoInternoColaborador)
        {
            var apiGenericResult = new ApiGenericResult<string>();

            try
            {
                if (string.IsNullOrEmpty(oneOnOneId))
                {
                    throw new ArgumentException("ID do one on one é obrigatório", nameof(oneOnOneId));
                }

                var sucesso = await _colaboradorRepository.InserirVisualizacaoOneOnOneAsync(oneOnOneId, codigoInternoColaborador);

                if (!sucesso)
                {
                    throw new ApplicationException($"One on One {oneOnOneId} não encontrado ou não pertence ao colaborador");
                }

                apiGenericResult.Retorno = "Visualização do one on one registrada com sucesso";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<InserirPautaSugeridaColaboradorResponseDTO>> InserirPautaSugeridaColaboradorAsync(string codigoInternoColaboradorAvaliado, InserirPautaSugeridaColaboradorRequestDTO request)
        {
            var apiGenericResult = new ApiGenericResult<InserirPautaSugeridaColaboradorResponseDTO>();

            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request não pode ser nulo");
                }

                if (string.IsNullOrEmpty(request.CodigoInternoColaboradorSuperior))
                {
                    throw new ArgumentException("Código interno do colaborador superior é obrigatório", nameof(request.CodigoInternoColaboradorSuperior));
                }

                var resultado = await _colaboradorRepository.UpsertPautaSugeridaColaboradorAsync(codigoInternoColaboradorAvaliado, request);

                apiGenericResult.Retorno = resultado;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }
    }
}
