using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.MapaAlocacao;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.MapaDeAlocacao;
using DataTransferObject.Domain.MapaDeAlocacao.ConsultaAlocacao;
using Foursys.Domain.Interfaces.Services;
using MapaDeAlocacao.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl
{
    [LogDomainClass]
    public class MapaDeAlocacaoValidadorService : IMapaDeAlocacaoValidadorService
    {
        private readonly IMapaAlocacaoRepository _mapaDeAlocacaoRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;

        private const string MENSAGEM_ERRO_ZERO_HORAS = "Para efetuar uma alocação, é necessário atribuir um valor igual ou superior a zero horas.";
        private const string MENSAGEM_ERRO_QUANTIDADE_MAXIMA_HORAS = "As horas não podem passar de um periodo de um dia.";

        private const int QUANTIDADE_MAXIMA_HORAS = 24;

        public MapaDeAlocacaoValidadorService(IMapaAlocacaoRepository mapaAlocacaoRepository,
                                               IUsuarioColaboradorRepository usuarioColaboradorRepository,
                                               IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService)
        {
            _mapaDeAlocacaoRepository = mapaAlocacaoRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
        }

        public async Task<bool> ValidarConflitoColaboradorAlocacaoEditar(DateTime dataInicio, DateTime dataFim, long periodoAlocacaoId, string codigoColaborador, string codigoProjeto, bool ehTbd, int orgId)
        {
            List<ListaConsultaDatasDTO> listaAlocacoes = await _mapaDeAlocacaoRepository.ConsultaDatasPeriodoAlocacaoColabOuTbd(codigoProjeto, codigoColaborador, periodoAlocacaoId, ehTbd, orgId);
            return VerificarConflitosInternos(dataInicio, dataFim, listaAlocacoes);
        }

        public async Task<bool> ValidarConflitoAlocacaoCadastrar(DateTime dataInicio, DateTime dataFim, string codigoProjeto, string codigoColaborador, bool ehTbd, int orgId)
        {
            List<ListaConsultaDatasDTO> listaAlocacoes = await _mapaDeAlocacaoRepository.ConsultaDatasPeriodoAlocacaoColabOuTbd(codigoProjeto, codigoColaborador, null, ehTbd, orgId);
            return VerificarConflitosInternos(dataInicio, dataFim, listaAlocacoes);
        }

        private bool VerificarConflitosInternos(DateTime dataInicio, DateTime dataFim, IEnumerable<ListaConsultaDatasDTO> listaAlocacoes)
        {
            foreach (ListaConsultaDatasDTO periodo in listaAlocacoes)
            {
                bool conflitoDataInicio = (dataInicio >= periodo.DataInicioPeriodo && dataInicio <= periodo.DataFimPeriodo);
                bool conflitoDataFim = (dataFim >= periodo.DataInicioPeriodo && dataFim <= periodo.DataFimPeriodo);
                bool conflitoPeriodoCompleto = (dataInicio <= periodo.DataInicioPeriodo && dataFim >= periodo.DataFimPeriodo);
                if (conflitoDataInicio || conflitoDataFim || conflitoPeriodoCompleto)
                {
                    return false; // Houve um conflito
                }
            }

            return true; // Não houve conflito
        }

        public void ValidaAcesso(string cpfSolicitante, int orgId)
        {
            // Verifica o acesso
            bool acessoValido = _mapaDeAlocacaoRepository.ValidaAcesso(cpfSolicitante, orgId);

            if (!acessoValido)
            {
                throw new UnauthorizedAccessException("Acesso negado");
            }
        }

        public bool ValidaSeExisteCodigoProjetoParaOrgId(string codigoProjeto, int orgId)
        {
            return _mapaDeAlocacaoRepository.ExisteCodigoProjetoParaOrgId(codigoProjeto, orgId);
        }

        public bool ValidarSeDataInicioEhMenorQueDataFimPeriodo(List<PeriodoDTO> listaPeriodo)
        {
            foreach (var periodo in listaPeriodo)
            {
                ValidarSeDataInicioEhMenorQueDataFimPeriodo(periodo);
            }

            return true;
        }

        public bool ValidarSeDataInicioEhMenorQueDataFimPeriodo(PeriodoDTO periodo)
        {
            if (!DateTimeUtil.VerificarSeDataFimEhMaiorOuIgualQueDataInicio(periodo.DataInicio, periodo.DataFim))
            {
                throw new ArgumentException("A data de início de um período não pode ser superior à data fim");
            }

            return true;
        }

        public async Task ValidaMapaAlocacaoCadastro(CadastroMapaAlocacaoValidacaoDTO cadastroMapaAlocacaoValidacao)
        {
            await ValidaMapaAlocacao(cadastroMapaAlocacaoValidacao, CRUDEnum.Create);
        }

        public async Task ValidaMapaAlocacaoEditar(long periodoAlocacaoId, DateTime? dataInicio, DateTime? dataFim, double? quantidadeDeHoras, string cpfRequest, int orgId)
        {
            var cadastroMapaAlocacaoValidacao = new CadastroMapaAlocacaoValidacaoDTO()
            {
                PeriodoAlocadoId = periodoAlocacaoId,
                DataInicio = dataInicio,
                DataFim = dataFim,
                QuantidadeHoras = quantidadeDeHoras,
                CpfSolicitante = cpfRequest,
                OrgIdLogada = orgId
            };

            var alocacao = _mapaDeAlocacaoRepository.GetColaboradorPeriodoAlocacaoById(cadastroMapaAlocacaoValidacao.PeriodoAlocadoId);

            cadastroMapaAlocacaoValidacao.CodigoProjeto = alocacao.CodigoProjeto;
            cadastroMapaAlocacaoValidacao.CpfColaborador = alocacao.CpfColaborador;
            cadastroMapaAlocacaoValidacao.CodigoColaborador = alocacao.CodigoColaborador;
            cadastroMapaAlocacaoValidacao.CodigoTbd = String.IsNullOrEmpty(alocacao.CodigoTbd) ? null : int.Parse(alocacao.CodigoTbd);
            cadastroMapaAlocacaoValidacao.OrgIdAlterada = alocacao.OrgId;

            await ValidaMapaAlocacao(cadastroMapaAlocacaoValidacao, CRUDEnum.Update);
        }

        private List<string> GetColaboradoresQuePermitemSobreposicao(int orgId, string cpf)
        {
            var parametroValor = _buscaParametroConfiguracaoService.GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.PERMITIR_SOBREPOSICAO_ALOCACOES, orgId, cpf);
            List<string> codigosColabsPermitemSobreposicao = StringUtil.SplitPontoEVirgula(parametroValor);
            return codigosColabsPermitemSobreposicao;
        }

        private async Task ValidaMapaAlocacao(CadastroMapaAlocacaoValidacaoDTO cadastroMapaAlocacaoValidacao, CRUDEnum acao)
        {
            if (cadastroMapaAlocacaoValidacao.QuantidadeHoras.ToInt() < 0)
            {
                throw new ArgumentException(MENSAGEM_ERRO_ZERO_HORAS);
            }

            var cadastroMapaAlocacaoFirstOrNull = cadastroMapaAlocacaoValidacao;

            if (cadastroMapaAlocacaoFirstOrNull is null)
            {
                throw new Exception("Periodo Alocado não foi preenchido corretamente!");
            }

            if (string.IsNullOrEmpty(cadastroMapaAlocacaoFirstOrNull.CpfSolicitante))
            {
                throw new Exception("CPF do solicitante precisa ser preenchido!");
            }

            if (!cadastroMapaAlocacaoFirstOrNull.IsColaborador && !cadastroMapaAlocacaoValidacao.IsTbd)
            {
                throw new Exception("É necessário ao menos um colaborador ou TBD para a validação.");
            }

            if (cadastroMapaAlocacaoValidacao.OrgIdLogada <= 0)
            {
                throw new Exception("É necessário informar a Org para validação.");
            }

            if (acao == CRUDEnum.Create)
            {
                if (cadastroMapaAlocacaoValidacao.IsColaborador)
                {
                    if (!_usuarioColaboradorRepository.ExisteColaborador(cadastroMapaAlocacaoValidacao.CpfColaborador))
                    {
                        throw new ApplicationException("O colaborador não está cadastrado.");
                    }

                    // Verificar se já existe um colaborador com o código de colaborador especificado na organização
                    if (!_usuarioColaboradorRepository.ExisteColaboradorComCodColaboradorExternoECpf(cadastroMapaAlocacaoValidacao.CodigoColaborador, cadastroMapaAlocacaoValidacao.CpfColaborador, cadastroMapaAlocacaoValidacao.OrgIdLogada))
                    {
                        throw new ApplicationException("Não foi encontrado nenhum registro com a combinação de Código Colaborador e CPF nesta organização.");
                    }
                }

                if (cadastroMapaAlocacaoValidacao.IsTbd && cadastroMapaAlocacaoValidacao.IsColaborador)
                {
                    throw new ArgumentException("Alocações de TBD e colaboradores devem ser criadas separadamente.");
                }
            }

            if (acao == CRUDEnum.Update)
            {
                if (cadastroMapaAlocacaoValidacao.OrgIdLogada != cadastroMapaAlocacaoValidacao.OrgIdAlterada)
                    throw new ApplicationException("Não é possível alterar a Alocação de uma outra Org.");
            }

            var permiteLancarMaisQue24Horas = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.PERMITIR_LANCAMENTO_MAIOR_24_HORAS, cadastroMapaAlocacaoValidacao.OrgIdLogada, cadastroMapaAlocacaoValidacao.CpfSolicitante);
            var codigosColabsPermitemSobreposicao = GetColaboradoresQuePermitemSobreposicao(cadastroMapaAlocacaoValidacao.OrgIdLogada, cadastroMapaAlocacaoValidacao.CpfSolicitante);

            if (!permiteLancarMaisQue24Horas)
            {
                if (cadastroMapaAlocacaoValidacao.QuantidadeHoras >= QUANTIDADE_MAXIMA_HORAS)
                {
                    throw new ApplicationException(MENSAGEM_ERRO_QUANTIDADE_MAXIMA_HORAS);
                }
            }

            ValidarSeDataInicioEhMenorQueDataFimPeriodo(new PeriodoDTO()
            {
                DataInicio = cadastroMapaAlocacaoValidacao.DataInicio.Value,
                DataFim = cadastroMapaAlocacaoValidacao.DataFim.Value
            });

            if (cadastroMapaAlocacaoValidacao.IsColaborador)
            {
                if (acao == CRUDEnum.Create)
                {
                    if (cadastroMapaAlocacaoValidacao.IsColaborador)
                    {
                        if (!codigosColabsPermitemSobreposicao.Contains(cadastroMapaAlocacaoValidacao.CodigoColaborador)
                            && !await ValidarConflitoAlocacaoCadastrar(cadastroMapaAlocacaoValidacao.DataInicio.Value, cadastroMapaAlocacaoValidacao.DataFim.Value, cadastroMapaAlocacaoValidacao.CodigoProjeto, cadastroMapaAlocacaoValidacao.CodigoColaborador, cadastroMapaAlocacaoValidacao.IsTbd, cadastroMapaAlocacaoValidacao.OrgIdLogada))
                        {
                            throw new ApplicationException("Já há uma alocação para o colaborador neste projeto que conflita com o período selecionado.");
                        }
                    }
                }
                if (acao == CRUDEnum.Update)
                {
                    if (cadastroMapaAlocacaoValidacao.DataInicio.HasValue() || cadastroMapaAlocacaoValidacao.DataFim.HasValue())
                    {
                        if (!codigosColabsPermitemSobreposicao.Contains(cadastroMapaAlocacaoValidacao.CodigoColaborador)
                            && !await ValidarConflitoColaboradorAlocacaoEditar(cadastroMapaAlocacaoValidacao.DataInicio.Value, cadastroMapaAlocacaoValidacao.DataFim.Value, cadastroMapaAlocacaoValidacao.PeriodoAlocadoId, cadastroMapaAlocacaoValidacao.CodigoColaborador, cadastroMapaAlocacaoValidacao.CodigoProjeto, cadastroMapaAlocacaoValidacao.IsTbd, cadastroMapaAlocacaoValidacao.OrgIdLogada))
                        {
                            throw new ApplicationException("Já há uma alocação para o colaborador neste projeto com o mesmo período.");
                        }
                    }
                }
            }
        }
    }
}