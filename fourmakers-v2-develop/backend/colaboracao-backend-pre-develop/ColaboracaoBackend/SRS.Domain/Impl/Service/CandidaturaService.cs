using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Colaborador;
using Core.Domain.Contratacao;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using DocumentFormat.OpenXml.Math;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using Social.Domain.Interfaces;
using SRS.Domain.Impl.Util;
using SRS.Domain.Interfaces.Service;
using SRS.Domain.Interfaces.Service.Validadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class CandidaturaService : ICandidaturaService
    {
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly string _pastaKey = "arquivos/candidaturas/";

        private readonly ICandidaturaValidatorService _candidaturaValidatorService;
        private readonly IDBConnectionUnitOfWork _connectionUnitOfWork;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IComentarioCandidaturaService _comentarioCandidatura;
        private readonly IVagaService _vagaService;

        private readonly IAspNetUser _aspNetUser;
        private readonly IEnvioEmail _envioEmail;
        private readonly ITemplateRepository _templateRepository;
        private readonly IContratacaoRepository _contratacaoRepository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;

        private static bool EmailEhValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }
        public CandidaturaService(ICandidaturaRepository candidaturaRepository, IDBConnectionUnitOfWork connectionUnitOfWork, ICandidaturaValidatorService candidaturaValidatorService, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository, IComentarioCandidaturaService comentarioCandidatura, IUploadFilesClient uploadFilesClient, IVagaService vagaService, IAspNetUser aspNetUser, ITemplateRepository templateRepository, IEnvioEmail envioEmail, IContratacaoRepository contratacaoRepository, IBuscaColaboradorRepository buscaColaboradorRepository)
        {
            _candidaturaRepository = candidaturaRepository;
            _connectionUnitOfWork = connectionUnitOfWork;
            _candidaturaValidatorService = candidaturaValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _comentarioCandidatura = comentarioCandidatura;
            _uploadFilesClient = uploadFilesClient;
            _vagaService = vagaService;
            _aspNetUser = aspNetUser;
            _envioEmail = envioEmail;
            _templateRepository = templateRepository;
            _contratacaoRepository = contratacaoRepository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
        }

        public async Task<ApiGenericResult<CandidaturaArquivosDTO>> InserirArquivo(CandidaturaArquivosParams param, string codigoInternoColaboradorLogado)
        {
            var data = DateTime.Now.ToString("yyyyMMddHHmm");
            var nomeBase = $"{data}_{param.NomeArquivo}";
            var objectKey = $"{_pastaKey}{nomeBase}";

            var bytes = param.bytes; // já é byte[]

            try
            {
                await _uploadFilesClient.UploadFile(objectKey, bytes);


                var linkUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(
                    EnvironmentVariables.SERVICE_MIDIA) + objectKey;


                var res = await _candidaturaRepository.InserirArquivo(param, linkUrl, codigoInternoColaboradorLogado);

                if (res == null)
                {
                    return new ApiGenericResult<CandidaturaArquivosDTO>
                    {
                        Sucesso = false,
                        Mensagem = "Não foi possível salvar o arquivo no banco.",
                        Retorno = null
                    };
                }

                return new ApiGenericResult<CandidaturaArquivosDTO>
                {
                    Erros = null,
                    Sucesso = true,
                    Mensagem = "Arquivo salvo com sucesso",
                    Retorno = res,
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível salvar o arquivo: {e.Message}", e);
            }
        }

        public async Task<ApiGenericResult<bool>> DeletarArquivo(string arquivoPath)
        {
            try
            {
                var status = await _uploadFilesClient.DeleteFile(arquivoPath);
                if (!status.Sucesso)
                {
                    return new ApiGenericResult<bool>
                    {
                        Sucesso = false,
                        Mensagem = "Erro ao deletar arquivo.",
                        Retorno = false
                    };
                }

                var res = await _candidaturaRepository.DeletarArquivo(arquivoPath);

                return new ApiGenericResult<bool>
                {
                    Sucesso = true,
                    Mensagem = "Arquivo deletado com sucesso.",
                    Retorno = res
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception($"Não foi possível deletar o arquivo: {e.Message}", e);
            }
        }

        public async Task AtualizarStatusCandidatura(int vagaId, int candidatoId, int statusId, string descricao, OrigemVagaEnum origem)
        {
            try
            {
                await _candidaturaValidatorService.ValidaCandidaturaStatus(vagaId, candidatoId, statusId, descricao, origem, CRUDEnum.Update);
                _connectionUnitOfWork.BeginTransaction();
                var verificaStatus = await _candidaturaRepository.BuscarStatusCandidaturaPorId(statusId);
                if (verificaStatus == null)
                {
                    await _candidaturaRepository.InserirTipoStatusCandidatura(statusId, descricao, origem.ToString());
                }

                await _candidaturaRepository.AlterarStatusCandidatura(vagaId, candidatoId, statusId);
                _connectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _connectionUnitOfWork.SafeRollback();
                throw ex;
            }
        }

        public async Task<List<CandidaturaStatusDTO>> ListarStatusCandidatura()
        {
            return await _candidaturaRepository.ListarStatusCandidatura();
        }

        public async Task<List<VagaCandidatoGestaoDTO>> ListarCandidatosDeVagas(string codColaborador, string pesquisa, string candidato, List<int> status, string titulo, int orgId)
        {
            ValidaAcessoCandidatura(codColaborador, orgId, FuncionalidadeSistemaEnum.VISUALIZAR_CANDIDATURAS_ORG);
            return await _candidaturaRepository.ListarCandidatosDeVagas(pesquisa, candidato, status, titulo, orgId);
        }

        private void ValidaAcessoCandidatura(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpf, orgId, funcionalidadesAcesso);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException("Acesso negado para Candidatura");
            }
        }

        public async Task<string> ReprovarCandidatura(string cpf, ReprovarCandidaturaParam param)
        {
            //if (!await _candidaturaRepository.EstaCandidaturaExiste(param.IdCandidatura))
            //    throw new ApplicationException("Id Candidatura invalido");

            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(param.IdCandidatura);
            if (candidatura == null)
                throw new ApplicationException("Id Candidatura não encontrada.");

            //Verificar se tem email do Candidato
            var dadosCandidato = await _candidaturaRepository.BuscarDadosColaboradorEmail(candidatura.CodColaborador, (int)candidatura.OrgId);

            if (dadosCandidato == null)
                throw new ApplicationException("Candidato não encontrado na base do Colaborador!");

            if (dadosCandidato.Email == null)
                throw new ApplicationException("Email do Candidato não cadastrado!");

            if (!EmailEhValido(dadosCandidato.Email))
                throw new ApplicationException($"O e-mail cadastrado para o candidato ({dadosCandidato.Email}) é inválido!");

            //Verificar se tem email do Reprovador
            var dadosReprovador = await _candidaturaRepository.BuscarDadosColaboradorEmail(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

            if (dadosReprovador == null)
                throw new ApplicationException("Código Colaborador do Reprovador nas bases do Colaborador não encontrado !");

            if (dadosReprovador.Email == null)
                throw new ApplicationException("Email do Reprovador não cadastrado !");

            if (!EmailEhValido(dadosReprovador.Email))
                throw new ApplicationException($"O seu e-mail cadastrado do Reprovador ({dadosReprovador.Email}) está em um formato inválido!");

            var orgDescricao = await _candidaturaRepository.BuscarDescricaoOrg(_aspNetUser.GetUsuarioLogado().OrgId);

            var motivo = _candidaturaRepository.ObterMotivoReprovacaoPorId(param.IdMotivoReprovacao) ?? throw new ApplicationException("Id motivo reprovacao invalido.");

            (string comentarioId, string msg) resultado = await _vagaService.MudarStatusCandidatura(param.IdCandidatura, StatusCandidaturaRecrutamento.Reprovado.ToInt(), cpf, param.Comentario);

            await _candidaturaRepository.ReprovarCandidato(param.IdCandidatura, param.IdMotivoReprovacao);

            //Envia email
            var dadosEmail = await _candidaturaRepository.BuscarDadosTemplateEmail(_aspNetUser.GetUsuarioLogado().OrgId);

            string mensagemCorpo = dadosEmail != null && !string.IsNullOrEmpty(orgDescricao?.DescricaoOrg) ? dadosEmail.Descricao
                : $@"<p>
                        Agradecemos pelo seu interesse e pelo envio do seu currículo.
                        Neste momento, não seguiremos com o seu perfil para o próximo passo do
                        processo seletivo para a vaga {candidatura.TituloVaga}. Seu currículo permanecerá
                        em nosso banco de talentos para futuras oportunidades compatíveis.
                        <br>
                    </p>
                    <p>Aproveite para atualizar o seu perfil: https://app.fourmakers.io/login<br></p>
                    <p>Desejamos sucesso na sua trajetória profissional.<br></p>
                    <p>Atenciosamente,<br>Equipe de Recursos Humanos</p>";

            // Define os e-mails em cópia
            string cCopia = dadosEmail?.EmailsCC ?? "";

            //if (dadosEmail != null)
            //{
            //    mensagemCorpo = mensagemCorpo.Replace("{vaga}", candidatura.TituloVaga);
            //}

            var assunto = !string.IsNullOrWhiteSpace(dadosEmail?.Titulo)
                ? dadosEmail.Titulo
                : $"Processo Seletivo - {candidatura.TituloVaga} - {dadosCandidato.DescricaoOrg}";

            _envioEmail.EnviaEmailCandidatoReprovado(
                dadosCandidato.NomeColaborador,
                dadosCandidato.Email,
                cCopia,
                assunto,
                mensagemCorpo
            );

            return resultado.msg;
        }

        public async Task<IEnumerable<CandidaturaLogDTO>> ListarLogsCandidaturaPorColaborador(string codColaborador)
        {
            return await _candidaturaRepository.ListarLogsCandidaturaPorColaborador(codColaborador);
        }

        public async Task<IEnumerable<CandidaturaAgrupadaDTO>> ListarLogsCandidaturaAgrupadosPorColaborador(string codColaborador, string busca, string idCandidatura = null)
        {
            var candidaturas = await _candidaturaRepository.ListarLogsCandidaturaAgrupadosPorColaborador(codColaborador, busca);

            if (idCandidatura is null)
                return candidaturas;

            return candidaturas.Where(m => m.IdCandidatura == idCandidatura);
        }

        public async Task AtualizarCandidatura(AtualizarCandidaturaParam param, string codigoInternoColaborador)
        {
            if (string.IsNullOrEmpty(param.IdCandidatura))
                throw new ArgumentException("Id da candidatura é obrigatório");

            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(param.IdCandidatura);
            if (candidatura is null)
                throw new ApplicationException("Candidatura não encontrada");

            if (!string.IsNullOrEmpty(param.ModeloTrabalhoId))
            {
                if (!await _candidaturaRepository.EstaModeloTrabalhoExiste(param.ModeloTrabalhoId))
                    throw new ApplicationException("Modelo de trabalho não encontrado");
            }

            if (!string.IsNullOrEmpty(param.DisponibilidadeEntrevistaId))
            {
                if (!await _candidaturaRepository.EstaDisponibilidadeEntrevistaExiste(param.DisponibilidadeEntrevistaId))
                    throw new ApplicationException("Disponibilidade de entrevista não encontrada");
            }

            var pretencaoSalarial = param.PretencaoSalarial is null ? candidatura.PretencaoSalarial : param.PretencaoSalarial;
            var modeloTrabalhoId = param.ModeloTrabalhoId ?? candidatura.ModeloTrabalhoId;
            var disponibilidadeEntrevistaId = param.DisponibilidadeEntrevistaId ?? candidatura.DisponibilidadeEntrevistaId;
            var quantidadeDiasPresencial = param.QuantidadeDiasPresencial is null ? candidatura.QuantidadeDiasPresencial : param.QuantidadeDiasPresencial;

            await _candidaturaRepository.AtualizarCandidatura(
                param.IdCandidatura,
                pretencaoSalarial,
                modeloTrabalhoId,
                disponibilidadeEntrevistaId,
                quantidadeDiasPresencial,
                codigoInternoColaborador,
                OrigemAlteracaoPretensaoModeloLogEnum.MovimentacaoKanban);
        }

        public async Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorCandidatura(Guid idCandidatura, string codigoUsuarioLogado, int limit, int cursor)
        {
            if (idCandidatura == Guid.Empty)
            {
                throw new ArgumentException("IdCandidatura inválido.");
            }

            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(idCandidatura.ToString()) ?? throw new ApplicationException("Candidatura não encontrada.");

            if (string.IsNullOrEmpty(candidatura.ColaboradorResponsavel) || !string.Equals(candidatura.ColaboradorResponsavel, codigoUsuarioLogado, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException("Somente o recrutador responsável pelo candidato tem acesso ao histórico.");
            }

            return await _candidaturaRepository.ListarLogPretensaoModeloPorCandidatura(idCandidatura, limit, cursor);
        }

        public async Task<List<LogPretensaoModeloCandidaturaDTO>> ListarLogPretensaoModeloPorOrganizacao(Guid idCandidatura, int orgId, int limit, int cursor, string codigoUsuarioLogado)
        {
            if (idCandidatura == Guid.Empty)
                throw new ArgumentException("Código da candidatura inválido.");

            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(idCandidatura.ToString()) ?? throw new ApplicationException("Candidatura não encontrada.");

            if (candidatura.OrgId != orgId)
                throw new ApplicationException("Candidatura não encontrada.");

            if (string.IsNullOrEmpty(candidatura.ColaboradorResponsavel) || !string.Equals(candidatura.ColaboradorResponsavel, codigoUsuarioLogado, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("Somente o recrutador responsável pelo candidato tem acesso ao histórico.");

            return await _candidaturaRepository.ListarLogPretensaoModeloPorOrganizacao(orgId, limit, cursor, candidatura.CodColaborador);
        }

        public async Task AtualizarRecrutadorResponsavel(string idCandidatura, string codigoRecrutadorResponsavel, string codigoUsuarioLogado)
        {
            if (string.IsNullOrEmpty(idCandidatura))
                throw new ArgumentException("Id da candidatura é obrigatório");

            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(idCandidatura);
            if (candidatura is null)
                throw new ApplicationException("Candidatura não encontrada");

            await _candidaturaRepository.AtualizarRecrutadorResponsavel(
                idCandidatura,
                codigoRecrutadorResponsavel,
                codigoUsuarioLogado);
        }

        public async Task<CandidaturaRecrutamentoDTO> ObterCandidaturaPorId(string idCandidatura)
        {
            return await _candidaturaRepository.ObterCandidaturaPorId(idCandidatura);
        }


        public async Task<PdfTemplateDTO> GetPdfTemplateHeaderPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetPdfTemplateHeaderPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }

        public async Task<ProfissionalDTO> GetProfissionalPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetProfissionalPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }

        public async Task<VagaAdmissaoDTO> GetVagaAdmissaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetVagaAdmissaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }
        public async Task<BeneficiosDTO> GetBeneficiosPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetBeneficiosPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }

        public async Task<ChecklistInstalacaoDTO> GetChecklistInstalacaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetChecklistInstalacaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }

        public async Task<AcessosUsuarioDTO> GetAcessosUsuarioPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetAcessosUsuarioPdfAdmissao(idCandidatura, codInternoCandidato, idVaga);
        }

        public async Task<PdfTemplateDTO> GetPdfTemplateDeAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetPdfTemplateDeAdmissaoPdf(idCandidatura, codInternoCandidato, idVaga);
        }
        public async Task<EquipamentosFoursysDTO> GetEquipamentoFourSysAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga)
        {
            return await _candidaturaRepository.GetEquipamentoFourSysAdmissaoPdf(idCandidatura, codInternoCandidato, idVaga);
        }
        public async Task<PdfTemplateDTO> CreatePdfTemplateDeAdmissao(CreatePdfTemplateParameters param)
        {
            if (string.IsNullOrEmpty(param.IdCandidatura))
                throw new ArgumentException("Id da candidatura é obrigatório");


            return await _candidaturaRepository.CreatePdfTemplateDeAdmissao(param);
        }

        public async Task<List<AcessoSistemaDTO>> GetSistemasLiberadosTemplatePdf()
        {
            return await _candidaturaRepository.GetSistemasLiberadosTemplatePdf();
        }

        public async Task<List<AcessoDiretorioRedeDTO>> GetAcessosPastaRedeTemplatePdf()
        {
            return await _candidaturaRepository.GetAcessosPastaRedeTemplatePdf();
        }

        public async Task<List<MotivoDeclinioDTO>> ListarMotivosDeclinio()
        {
            return await _candidaturaRepository.ListarMotivosDeclinio();
        }

        public async Task<List<MotivoReprovacaoDTO>> ListarMotivosReprovacao()
        {
            return await _candidaturaRepository.ListarMotivosReprovacao();
        }

        public async Task<MotivoDeclinioDTO> ObterMotivoDeclinioPorId(string id)
        {
            return await _candidaturaRepository.ObterMotivoDeclinioPorId(id);
        }

        public async Task<MotivoReprovacaoDTO> ObterMotivoReprovacaoPorId(string id)
        {
            return await _candidaturaRepository.ObterMotivoReprovacaoPorId(id);
        }

        public async Task<string> DeclinarCandidato(DeclinarCandidatoParam param, string codColaborador)
        {
            var motivos = _candidaturaRepository.ObterMotivoDeclinioPorId(param.IdMotivoDeclinio);
            if (motivos == null)
                throw new ApplicationException("Id Motivo invalido.");

            (string comentarioId, string msg) resultado = await _vagaService.MudarStatusCandidatura(param.IdCandidatura, StatusCandidaturaRecrutamento.Declinou.ToInt(), codColaborador, param.Comentario);

            await _candidaturaRepository.DeclinarCandidato(param.IdCandidatura, param.IdMotivoDeclinio);

            return resultado.msg;
        }

        public async Task<List<MeusTalentos>> ListarMeusTalentos(string codigoUsuarioLogado)
        {
            return await _candidaturaRepository.ListarMeusTalentos(codigoUsuarioLogado);
        }

        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailInserir(CandidatoTemplateEmailParamDTO param)
        {
            var result = await _candidaturaRepository.CandidatoTemplateEmailInserir(param);

            if (result == null)
                throw new InvalidOperationException("Erro ao inserir Template da Org.");

            return result;
        }

        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailAtualizar(CandidatoTemplateEmailParamDTO param)
        {
            var result = await _candidaturaRepository.CandidatoTemplateEmailAtualizar(param);

            if (result == null)
                throw new InvalidOperationException("Template da Org não localizado.");

            return result;

        }

        public async Task<bool> CandidatoTemplateEmailDeletar(int orgId)
        {
            return await _candidaturaRepository.CandidatoTemplateEmailDeletar(orgId);
        }

        public async Task<CandidatoTemplateEmailResponseDTO> CandidatoTemplateEmailListarPorId(int orgId)
        {
            return await _candidaturaRepository.CandidatoTemplateEmailListarPorId(orgId);
        }

        public async Task<ApiGenericResult<DashboardBigNumbers>> dashboardMetricasRecrutamento(DashboardBigNumbersParam param)
        {
            var result = new ApiGenericResult<DashboardBigNumbers>();

            try
            {
                var list = await _candidaturaRepository.dashboardMetricasRecrutamento(param, _aspNetUser.GetUsuarioLogado().OrgId);

                return list == null ? new ApiGenericResult<DashboardBigNumbers>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum item encontrado."

                } : new ApiGenericResult<DashboardBigNumbers>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Big Number: {e.Message}");

            }

        }

        public async Task<ApiGenericResult<List<DashboardVagasEmFocoResponse>>> dashboardMetricasVagasEmFoco(DashboardBigNumbersParam param)
        {
            var result = new ApiGenericResult<List<DashboardVagasEmFocoResponse>>();

            try
            {
                var list = await _candidaturaRepository.dashboardMetricasVagasEmFoco(param, _aspNetUser.GetUsuarioLogado().OrgId);

                return list == null ? new ApiGenericResult<List<DashboardVagasEmFocoResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum item encontrado."

                } : new ApiGenericResult<List<DashboardVagasEmFocoResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Vagas em Foco: {e.Message}");

            }
        }
        public async Task<ApiGenericResult<DashboardFunilVagasResponse>> dashboardMetricasFunilDeVagas(DashboardBigNumbersParam param)
        {
            var result = new ApiGenericResult<DashboardFunilVagasResponse>();

            try
            {
                var list = await _candidaturaRepository.dashboardMetricasFunilDeVagas(param, _aspNetUser.GetUsuarioLogado().OrgId);

                return list == null ? new ApiGenericResult<DashboardFunilVagasResponse>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum item encontrado."

                } : new ApiGenericResult<DashboardFunilVagasResponse>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Funil de Vagas: {e.Message}");

            }

        }


        public async Task<ApiGenericResult<List<DashboardPerdidasMotivoResponse>>> dashboardMetricasVagasPerdidasMotivo(DashboardBigNumbersParam param)
        {
            var result = new ApiGenericResult<List<DashboardPerdidasMotivoResponse>>();

            try
            {
                var list = await _candidaturaRepository.dashboardMetricasVagasPerdidasMotivo(param, _aspNetUser.GetUsuarioLogado().OrgId);

                return list == null ? new ApiGenericResult<List<DashboardPerdidasMotivoResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum item encontrado."

                } : new ApiGenericResult<List<DashboardPerdidasMotivoResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Vagas Perdidas: {e.Message}");

            }
        }
        public async Task<ApiGenericResult<List<DashboardAquisicaoCandidatos>>> dashboardMetricasAquisicaoCandidatos(DashboardBigNumbersParam param)
        {
            var result = new ApiGenericResult<List<DashboardAquisicaoCandidatos>>();

            try
            {
                var list = await _candidaturaRepository.dashboardMetricasAquisicaoCandidatos(param, _aspNetUser.GetUsuarioLogado().OrgId);

                List<DashboardAquisicaoCandidatos> retorno = null;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        //BuscaColaboradorPorCodigoExterno
                        var organizacoes = await _buscaColaboradorRepository.GetOrganizacoesColaborador(item.CodigoInternoColaborador);
                        if (organizacoes == null || !organizacoes.Any())
                        {
                            item.Origem = "Outros";
                        }
                        else
                        {
                            var origem = CandidatoOrigemUtil.ColaboradorBancoTalentos(organizacoes);
                            item.Origem = string.IsNullOrEmpty(origem) ? "Outros" : origem;
                        }
                    }

                    retorno = list
                        .GroupBy(x => x.Origem ?? "Outros")
                        .Select(g =>
                        {
                            var primeiro = g.First();
                            return new DashboardAquisicaoCandidatos
                            {
                                Total = g.Sum(x => x.Total),
                                OrgId = primeiro.OrgId,
                                CodigoInternoColaborador = null,
                                Origem = g.Key
                            };
                        })
                        .ToList();
                }

                return retorno == null ? new ApiGenericResult<List<DashboardAquisicaoCandidatos>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum item encontrado."

                } : new ApiGenericResult<List<DashboardAquisicaoCandidatos>>
                {
                    Retorno = retorno,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Aquisição de Candidatos: {e.Message}");

            }

        }

        public async Task<ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>> dashboardNovosCandidatosPorOrigem(DashboardNovosCandidatosParam param)
        {
            var result = new ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>();
            try
            {
                var list = await _candidaturaRepository.dashboardNovosCandidatosPorOrigem(param, _aspNetUser.GetUsuarioLogado().OrgId);
                List<DashboardAquisicaoCandidatosResponse> retorno = null;
                if (list != null)
                {
                    retorno = list
                        .Select(item => new { Item = item, Origem = MapOrigemNovosCandidatos(item.TipoCadastro, item.FormaCadastro) })
                        .GroupBy(x => x.Origem ?? "Outros")
                        .Select(g =>
                        {
                            var primeiro = g.First();
                            return new DashboardAquisicaoCandidatosResponse
                            {
                                Total = g.Sum(x => x.Item.Total),
                                OrgId = primeiro.Item.OrgId,
                                Origem = g.Key
                            };
                        })
                        .ToList();
                }

                return retorno == null ? new ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = "Nenhum item encontrado."
                } : new ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>
                {
                    Retorno = retorno,
                    Sucesso = true,
                    Mensagem = "Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Novos Candidatos por Origem: {e.Message}");
            }
        }
        
        public async Task<ApiGenericResult<List<RecrutadorListagemResponse>>> RecrutadorListagem(string nomeRecrutador, int cursor, int limite)
        {

            if (cursor < 0)
                throw new ArgumentException("Cursor é obrigatório");

            if (limite <= 0)
                throw new ArgumentException("Limite é obrigatório");

            var result = new ApiGenericResult<List<RecrutadorListagemResponse>>();

            try
            {
                var list = await _candidaturaRepository.RecrutadorListagem(nomeRecrutador, cursor, limite, _aspNetUser.GetUsuarioLogado().OrgId);

                return list == null ? new ApiGenericResult<List<RecrutadorListagemResponse>>
                {
                    Retorno = null,
                    Sucesso = false,
                    Mensagem = $"Nenhum Recrutador encontrado."

                } : new ApiGenericResult<List<RecrutadorListagemResponse>>
                {
                    Retorno = list,
                    Sucesso = true,
                    Mensagem = $"Listagem obtida com sucesso."
                };
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                throw new Exception($"Erro ao listar Recrutadores: {e.Message}");

            }
        }

        public async Task<IEnumerable<ListarCandidaturasPorCodCandidatoResult>> BuscarCandidaturasColaborador(string codColaborador)
        {
            if (string.IsNullOrWhiteSpace(codColaborador))
                throw new ArgumentException("O código do colaborador é obrigatório.");

            return await _candidaturaRepository.ListarCandidaturasPorCodCandidato(codColaborador);
        }

        private static string MapOrigemNovosCandidatos(string tipoCadastro, int? formaCadastro)
        {
            if (!string.IsNullOrEmpty(tipoCadastro))
            {
                switch (tipoCadastro.ToUpperInvariant())
                {
                    case "SRS_LINKEDIN": return "Linkedin";
                    case "FOURMAKERS_LOTE": return "Importação";
                    case "FOURMAKERS_UNITARIO": return "Cadastro manual";
                    case "CADASTRO_DO_COLABORADOR": return "Cadastro do colaborador";
                }
            }
            if (formaCadastro.HasValue)
            {
                switch (formaCadastro.Value)
                {
                    case 1: return "Currículo";
                    case 2: return "Linkedin";
                    case 3: return "Cadastro manual";
                }
            }
            return "Outros";
        }
    }
}