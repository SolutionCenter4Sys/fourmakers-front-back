using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Core.Domain;
using Core.DomainModel;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Irony.Parsing;
using SRS.Domain.Interfaces.Service;
using SRS.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services;

using Logs.Infra.Attributes;

namespace SRS.Domain.Impl.Service
{
    [LogDomainClass]
    public class SRSService : ISRSService
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly ISRSClient _srsClient;
        private readonly ISRSRepository _sRSRepository;
        private readonly ITokens _token;
        private readonly IUsuarioClient _usuarioClient;
        private readonly IUsuarioService _usuarioService;
        private IUploadFilesClient _uploadFilesClient;
        private readonly ISRSInfraClient _srsInfra;
        private readonly ISRSColaboracaoClient _sRSColaboracaoClient;
        private readonly IDBConnectionUnitOfWork _dBConnectionUnitOfWork;
        private readonly ICandidaturaRepository _candidaturaRepository;

        public SRSService(IAspNetUser aspNetUser, ISRSClient srsClient, ISRSRepository sRSRepository, ITokens token, IUsuarioClient usuarioClient, IUploadFilesClient uploadFilesClient, IUsuarioService usuarioService, ISRSInfraClient srsInfra, ISRSColaboracaoClient sRSColaboracaoClient, IDBConnectionUnitOfWork dBConnectionUnitOfWork, ICandidaturaRepository candidaturaRepository)
        {
            _aspNetUser = aspNetUser;
            _srsClient = srsClient;
            _sRSRepository = sRSRepository;
            _token = token;
            _usuarioClient = usuarioClient;
            _usuarioService = usuarioService;
            _uploadFilesClient = uploadFilesClient;
            _srsInfra = srsInfra;
            _sRSColaboracaoClient = sRSColaboracaoClient;
            _dBConnectionUnitOfWork = dBConnectionUnitOfWork;
            _candidaturaRepository = candidaturaRepository;
        }

        public SRSResult Buscarvagas(string busca, int cursor, int limite, FiltroStatusPublicacaoEnum filtroStatusPublicacao)
        {
            try
            {
                var ret = new SRSResult();
                var orgId = 1;
                if (_aspNetUser.GetUsuarioLogado() != null)
                    orgId = _aspNetUser.GetUsuarioLogado().OrgId;
                ret.SrsDTO = _sRSRepository.BuscarVagas(busca, cursor, limite, filtroStatusPublicacao, orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public SkillsVagasResult BuscarSkillsVagas(List<long> id)
        {
            try
            {
                var ret = new SkillsVagasResult();
                ret.Skills = _sRSRepository.BuscarSkills(id);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public FavoritarVagasResult FavoritarVagas(long id_vaga, long tb_usuario_id)
        {
            try
            {
                var ret = new FavoritarVagasResult();
                var result = _sRSRepository.FavoritarVagas(id_vaga, tb_usuario_id);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public DesfavoritarResult DesfavoritarVagas(long id_vaga, long tb_usuario_id)
        {
            try
            {
                var ret = new DesfavoritarResult();
                var result = _sRSRepository.DesfavoritarVagas(id_vaga, tb_usuario_id);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public FavoritarVagasResult ListarVagasFavoritadas(long tb_usuario_id)
        {
            try
            {
                var ret = new FavoritarVagasResult();
                ret.VagasFavoritadas = _sRSRepository.ListarVagasFavoritadasPorUsuario(tb_usuario_id);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<SRSResult> BuscarRazao()
        {
            try
            {
                var retorno = new SRSResult();

                var token = await _srsClient.Autenticacao();
                retorno.Data = await _srsClient.RequisicaoRazao(token.token);
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public DetalheResult BuscarVagaDetalhada(long id_vaga)
        {
            try
            {
                var ret = new DetalheResult();
                ret.Detalhe = _sRSRepository.BuscarVagaDetalhada(id_vaga).ToList();
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<InscricoesCandidatoResult> CandidatoInscritoNasVagas()
        {
            try
            {
                var retorno = new InscricoesCandidatoResult();

                var token = await _srsClient.Autenticacao();
                var colaborador = (await _usuarioService.ShowMe(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId)).Colaborador;
                string cpf = colaborador.DocumentoColaborador;
                if (String.IsNullOrEmpty(cpf))
                    return retorno;
                var candidato = await _srsClient.GetCandidate(token.token, cpf);
                if (candidato.Sucess)
                {
                    if (candidato.Message.ToLower() == "candidate not found")
                    {
                        await _srsClient.PostCandidate(token.token, cpf, new SRSCandidateDTO
                        {
                            first_name = colaborador.NomeCompleto,
                            phone_home = "",
                            phone_cell = colaborador.ContatoPrincipal,
                            address = "",
                            address_number = "",
                            address_complement = "",
                            district = "",
                            city = "",
                            state = "",
                            zip = "",
                            source = "4Makers",
                            key_skills = null,
                            methodologies = null,
                            email1 = _aspNetUser.GetUsuarioLogado().Email,
                            email2 = "",
                            emailFoursys = "",
                            desired_pay = 0,
                            current_pay = 0,
                            cand_rg = "",
                            cand_estCivil = "",
                            cand_Lkdin = "",
                            cand_skype = "",
                            cand_gruporisco = "",
                            instagram = "",
                            facebook = "",
                            twitter = "",
                            cand_filhos = 0,
                            dataNascimento = null,
                            disponibilidade = "",
                            zona = "",
                            pcd = 0,
                            genre = "",
                            sexual_orientation = "",
                            ethnicity = "",
                            school_level = "",
                            refugee_person = 0
                        }, null);
                        candidato = await _srsClient.GetCandidate(token.token, cpf);
                    }
                }
                retorno.Data = await _srsClient.CandidatoInscritoNasVagas(token.token, candidato.SRSCandidateDTO.candidate_id);
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<CandidatarResult> CandidatarSe(long joborder_id, string convite, string origem = "4Makers", int? candidateId = 0, int? userId = null)
        {
            try
            {
                var retorno = new CandidatarResult();
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (origem == "4Makers")
                {
                    var authSRS = await _srsClient.Autenticacao();

                    var usuario = await _usuarioService.ShowMeSSOCandidato(usuarioLogado.Token, userId);

                    var documentoColaborador = usuario.Colaborador.DocumentoColaborador;
                    var candidato = await _srsClient.GetCandidate(authSRS.token, documentoColaborador);
                    retorno = await _srsClient.CandidatarSeAUmaVaga(authSRS.token, joborder_id, candidato.SRSCandidateDTO.candidate_id, origem);
                    if (retorno.Sucess)
                    {
                        await _sRSRepository.InserirVagaCandidato(usuarioLogado.Cpf, joborder_id, usuarioLogado.OrgId, candidato.SRSCandidateDTO.candidate_id, 1);
                        await _candidaturaRepository.InserirLogAlteracaoStatusCandidatura((int)retorno.Data.Joborder_Id, (int)retorno.Data.Candidate_Id, 1);
                    }
                    else
                    {
                        throw new Exception("Não foi possivel candidatar-se");
                    }
                }
                else
                {
                    var token = await _srsClient.Autenticacao();
                    retorno = await _srsClient.CandidatarSeAUmaVaga(token.token, joborder_id, Convert.ToInt32(candidateId), origem);
                }

                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DescandidatarResult> DescandidatarSe(long joborder_id, int status_reason_cancellation, int reason_cancellation)
        {
            try
            {
                var retorno = new DescandidatarResult();

                var token = await _srsClient.Autenticacao();
                string cpf = _aspNetUser.GetUsuarioLogado().Cpf;
                string origem = "4Makers";
                var usuario = await _usuarioService.ShowMeSSOCandidato(_aspNetUser.GetUsuarioLogado().Token);
                var documentoColaborador = usuario.Colaborador.DocumentoColaborador;
                var candidato = await _srsClient.GetCandidate(token.token, documentoColaborador);
                retorno = await _srsClient.DescandidatarSeDeUmaVaga(token.token, joborder_id, candidato.SRSCandidateDTO.candidate_id, origem, status_reason_cancellation, reason_cancellation);

                if (retorno.Sucesso == true)
                {
                    _sRSRepository.RemoverVagaIndicada(joborder_id, cpf);
                    await _sRSRepository.RemoverVagaCandidato(candidato.SRSCandidateDTO.candidate_id, joborder_id);
                }
                return retorno;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private string GetVanityName(string linkedinUrl)
        {
            try
            {
                Regex validateDateRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
                if (!validateDateRegex.IsMatch(linkedinUrl))
                    throw new Exception();
                if (linkedinUrl.ElementAt(linkedinUrl.Length - 1) == '/')
                    linkedinUrl = linkedinUrl.Substring(0, linkedinUrl.Length - 1);
                return linkedinUrl.Split("/").Last();
            }
            catch (Exception)
            {
                throw new ValidationException("Url inválida");
            }
        }

        public async Task<string> GerarConvite(long vagaId, long idTbIndicacaoParc, string telefone, string nome, string linkedin, UsuarioColaboradorDTO usuario)
        {
            try
            {
                var vaga = BuscarVagaDetalhada(vagaId);
                if (vaga == null)
                {
                    throw new Exception("Vaga não encontrada!");
                }
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var authSRS = await _srsClient.Autenticacao();
                usuario = await _usuarioService.ShowMeSSOCandidato(usuarioLogado.Token);

                string idVaga = vagaId.ToString();
                string dataCriacao = DateTime.UtcNow.ToString();
                string convite = "";
                var urlBase = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE);
                var url = urlBase + "/vaga/candidatar/" + idVaga;
                convite = _token.Base64(usuario.Email + "|" + idVaga + "|" + dataCriacao + "|" + idTbIndicacaoParc + "|" + telefone + "|" + nome + "|" + GetVanityName(linkedin));
                _sRSRepository.InserirVagaIndicada(long.Parse(idVaga), DateTime.Now, GetVanityName(linkedin), nome, usuario.Email, url, usuarioLogado.OrgId);
                url += "/" + convite;
                return url;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string[] DesembrulharConvite(string convite)
        {
            convite = _token.DecodeBase64(convite);

            string[] conviteSplit = convite.Split("|");
            return conviteSplit;
        }

        public List<RecomendarVagasDTO> ListarVagasRecomendadas(string cpfSolicitante)
        {
            try
            {
                List<RecomendarVagasDTO> ret = new List<RecomendarVagasDTO>();

                var skills = _sRSRepository.GetSkillsUsuario(cpfSolicitante);

                var aux = _sRSRepository.ListarVagasRecomendadas(skills);

                double forcaVaga = 0.0;
                long vagaId = 0;

                foreach (var vaga in aux)
                {
                    if (vagaId == 0 || vagaId == vaga.VagasSrsId)
                    {
                        if (vagaId == 0)
                        {
                            ret.Add(vaga);
                        }

                        forcaVaga += 1;
                        vagaId = vaga.VagasSrsId;
                    }
                    else
                    {
                        ret.First(x => x.VagasSrsId == vagaId).compatibilidadeVaga = (forcaVaga / skills.Count()) * 100;
                        ret.Add(vaga);
                        vagaId = vaga.VagasSrsId;
                        forcaVaga = 1;
                    }
                }

                return ret.OrderByDescending(x => x.compatibilidadeVaga).ToList();
            }
            catch
            {
                throw;
            }
        }

        public long BuscaQuantidadeVaga()
        {
            try
            {
                return _sRSRepository.BuscaQuatidadeVagas();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public VagasIndicadasResult ListarVagasIndicadas()
        {
            try
            {
                var ret = new VagasIndicadasResult();
                ret.vagasIndicadas = _sRSRepository.ListarVagasIndicadas(_aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IdIndicacaoUsuario> IndicarVaga(long idVaga, string nome, string email, string telefone, string deOndeConhece, bool autorizou, bool estaDisponivel, string linkedin)
        {
            try
            {
                var ret = new IdIndicacaoUsuario();
                GetVanityName(linkedin);
                var srsCandidate = await _sRSColaboracaoClient.CadastroCandidatoFourmakersLinkedin(
                    VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO),
                    new CadastroCandidatoLinkedinInput
                    {
                        Email = email,
                        Telefone = telefone,
                        UrlLinkedin = linkedin
                    });
                if (_sRSRepository.CandidatoIndicadoVaga(idVaga, GetVanityName(linkedin)))
                    throw new ValidationException("Este candidato já foi indicado anteriormente. O cadastro do mesmo foi atualizado em nossa base de dados.");
                ret.Usuario = await _usuarioClient.ShowMe(_aspNetUser.GetUsuarioLogado().Token);
                ret.IdIndicacao = _sRSRepository.IndicarVaga(ret.Usuario.UsuarioId, idVaga, nome, email, telefone, deOndeConhece, autorizou, estaDisponivel, GetVanityName(linkedin));
                var authSRS = await _srsClient.Autenticacao();
                var retorno = await _srsClient.CandidatarSeAUmaVaga(authSRS.token, idVaga, srsCandidate.Retorno, "4Makers");

                return ret;
            }
            catch (ValidationException err)
            {
                throw new ValidationException(err.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddConviteIndicacaoParcial(long idTb, string convite, byte[] curriculo, TipoCertificadoEnum tipo)
        {
            var path = idTb + "_CURRICULO/";
            var PathCurriculo = "";
            //PATH_CURRIC:  arquivos/curriculos/
            var pathPasta = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CURRICULO) + path;
            var data = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            var nomeArquivo = String.Empty;
            if (curriculo != null)
            {
                if (tipo == TipoCertificadoEnum.IMAGEM)
                {
                    nomeArquivo = data + ".png";
                    await _uploadFilesClient.UploadFile(pathPasta + nomeArquivo, curriculo);
                }
                else
                {
                    var nomeThumb = data + "_thumb.jpg";
                    nomeArquivo = data + ".pdf";

                    await _uploadFilesClient.UploadFile(pathPasta + nomeArquivo, curriculo);

                    var thumbCurriculo = GeradorThumbUtil.ConverterPDF(curriculo, 150);

                    await _uploadFilesClient.UploadFile(pathPasta + nomeThumb, thumbCurriculo);
                }

                PathCurriculo = pathPasta + nomeArquivo;
            }

            _sRSRepository.AddConviteIndicacaoParcial(idTb, convite, PathCurriculo);
        }
        public async Task<List<VagaDTO>> BuscarVagaSRS(string token)
        {
            try
            {
                var vagas = await _sRSColaboracaoClient.BuscarVagaSRS(token);

                return vagas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro : {ex.Message}");
            }
        }
        public async Task<CriarVagasSRSParam> EditarCriarVagasFourmakersSRS(CriarVagasSRSParam param, string token)
        {
            UsuarioColaboradorDTO user;

            try
            {
                user = await _usuarioClient.ShowMe(token);
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException(e.Message);
            }

            var userId = _srsInfra.BuscaUserId(user.Email);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Usuário para criação de vaga não encontrado. Procure a área de Recrutamento & Seleção.");
            }

            param.IdUsuario = userId.ToString();

            var mensagemValidacao = ValidarParametros(param);

            if (mensagemValidacao != string.Empty)
            {
                throw new ArgumentException(mensagemValidacao);
            }

            return await _srsInfra.CriarOuAtualizarVagaFourmakersSRS(param);
        }

        public async Task<CriarVagasSRSParam> ObterVagaPorId(string token, int vagaId)
        {
            UsuarioColaboradorDTO user;

            try
            {
                user = await _usuarioClient.ShowMe(token);
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException("Erro ao obter informações do token!");
            }

            var userId = _srsInfra.BuscaUserId(user.Email);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Usuário para criação de vaga não encontrado. Procure a área de Recrutamento & Seleção.");
            }

            if (vagaId.ToIntOuZero() <= 0)
            {
                throw new ArgumentException("Parâmetro vagaId obrigatório.");
            }

            return await _srsInfra.ObterVagaPorId(vagaId);
        }

        private static string ValidarParametros(CriarVagasSRSParam param)
        {
            var mensagem = string.Empty;

            var camposFaltando = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(param.Unidade))
                camposFaltando.Add("Unidade", "Unidade não preenchida.");
            if (string.IsNullOrEmpty(param.Cargo))
                camposFaltando.Add("Cargo", "Cargo não preenchido.");
            if (string.IsNullOrEmpty(param.DescricaoCargo))
                camposFaltando.Add("DescricaoCargo", "Descrição do Cargo não preenchida.");
            if (string.IsNullOrEmpty(param.Titulo))
                camposFaltando.Add("Titulo", "Título não preenchido.");
            if (param.TipoLocalizacao != "HOME OFFICE" && string.IsNullOrEmpty(param.Estado))
                camposFaltando.Add("Estado", "Estado não preenchido.");
            if (param.TipoLocalizacao != "HOME OFFICE" && string.IsNullOrEmpty(param.Cidade))
                camposFaltando.Add("Cidade", "Cidade não preenchida.");
            if (string.IsNullOrEmpty(param.TipoLocalizacao))
                camposFaltando.Add("TipoLocalizacao", "Tipo de Localização não preenchido.");
            else if (param.TipoLocalizacao == "HÍBRIDO" && string.IsNullOrEmpty(param.Frequencia))
                camposFaltando.Add("Frequencia", "Frequencia não preenchida.");
            if (param.DataPrevistaInicio == DateTime.MinValue)
                camposFaltando.Add("DataPrevistaInicio", "Data Prevista de Início não preenchida.");
            if (string.IsNullOrEmpty(param.Aprovador))
                camposFaltando.Add("Aprovador", "Aprovador não preenchido.");
            if (string.IsNullOrEmpty(param.CargaHoraria))
                camposFaltando.Add("CargaHoraria", "Carga Horária não preenchida.");
            if (string.IsNullOrEmpty(param.Tipo))
                camposFaltando.Add("Tipo", "Tipo não preenchido.");
            if (string.IsNullOrEmpty(param.Solicitante))
                camposFaltando.Add("Solicitante", "Solicitante não preenchido.");
            if (string.IsNullOrEmpty(param.Termometro))
                camposFaltando.Add("Termometro", "Termômetro não preenchido.");
            if (string.IsNullOrWhiteSpace(param.MaquinaCliente) && string.IsNullOrWhiteSpace(param.MaquinaFour))
            {
                camposFaltando.Add("MaquinaFour ou MaquinaCliente", "Máquina Foursys ou Máquina Cliente não selecionada.");
            }
            if (!string.IsNullOrWhiteSpace(param.MaquinaFour))
            { 
                if (string.IsNullOrEmpty(param.StackPrincipal))
                    camposFaltando.Add("StackPrincipal", "Stack Principal não preenchida.");
                if (string.IsNullOrEmpty(param.ConfiguracaoMaquina))
                    camposFaltando.Add("ConfiguracaoMaquina", "Configuração da Máquina não preenchida.");
            }
            if (string.IsNullOrEmpty(param.DuracaoContrato))
                camposFaltando.Add("DuracaoContrato", "Duração do Contrato não preenchida.");
            if (param.TipoContratacao == null || param.TipoContratacao.Count == 0)
                camposFaltando.Add("TipoContratacao", "Tipo de Contratação não preenchido.");
            if (param.NumeroDeVagas <= 0)
                camposFaltando.Add("NumeroDeVagas", "Número de Vagas deve ser maior que zero.");
            if (param.Company == null)
                camposFaltando.Add("Company", "Dados da Empresa não preenchidos.");
            if (param.Skills == null || param.Skills.Count == 0)
                camposFaltando.Add("Skills", "Pelo menos uma Skill deve ser informada.");
            if (param.TaxaMaximaPorHora < 0)
                camposFaltando.Add("TaxaMaximaPorHora", "Taxa máxima por hora inválida.");

            if (camposFaltando.Count > 0)
            {
                mensagem = "Preencha os campos obrigatórios.";
                mensagem += "\nOs seguintes campos não foram preenchidos:";
                foreach (var campo in camposFaltando)
                {
                    mensagem += $"\n{campo.Key}: {campo.Value}";
                }
            }

            return mensagem;
        }

        public async Task<SkillVagaParam> EditarSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            return await _srsInfra.EditarSkillVagaFourmakersSRS(param);
        }

        public async Task<SkillVagaParam> InserirSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            return await _srsInfra.InserirSkillVagaFourmakersSRS(param);
        }

        public async Task<SkillVagaParam> RemoverSkillVagaFourmakersSRS(int skillId)
        {
            return await _srsInfra.RemoverSkillVagaFourmakersSRS(skillId);
        }

        public async Task<List<SkillVagaParam>> BuscaSkillsVagaFourmakersSRS(int vagaId)
        {
            return await _srsInfra.BuscaSkillsVagaFourmakersSRS(vagaId);
        }

        public async Task<bool> GetColaboradorAtivoOuInativoPorCPF(string colaboradorCpf, string token)
        {
            return await _sRSColaboracaoClient.GetColaboradorAtivoOuInativoPorCPF(colaboradorCpf, token);
        }

        public async Task<bool> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param)
        {
            return await _srsInfra.AlterarCategoriaHabilidade(param);
        }

        public async Task<List<IndicacaoPremiadaParcialDTO>> ListarIndicacoesPremiadas(ListarIndicacaoPremiadaParcialParam param)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            await ValidaAcessoGrupoFuncionalidade(usuarioLogado.Cpf, FuncionalidadeSistemaEnum.VISUALIZAR_INDICACOES);
            try
            {
                var ret = await _sRSRepository.ListarIndicacoesPremiadas(param, usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IndicacaoPremiadaParcialDTO> EditarIndicacaoPremiadaParcial(EditarIndicacaoPremiadaParcialParam param)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            await ValidaAcessoGrupoFuncionalidade(usuarioLogado.Cpf, FuncionalidadeSistemaEnum.EDITAR_INDICACOES);

            var verificaIndicacaoExistente = await _sRSRepository.VerificaSeExisteIndicacaoPremiadaParcial(param.Id);
            if (!verificaIndicacaoExistente)
            {
                throw new Exception("A Indicacao não existe.");
            }

            try
            {
                _dBConnectionUnitOfWork.BeginTransaction();
                var result = await _sRSRepository.EditarIndicacaoPremiadaParcial(param, usuarioLogado.OrgId);
                _dBConnectionUnitOfWork.Commit();
                return result;
            }
            catch (Exception e)
            {
                _dBConnectionUnitOfWork.Rollback();
                throw;
            }
        }

        public async Task<IndicacaoPremiadaParcialDTO> BuscarIndicacaoPremiadaPorId(int id)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            await ValidaAcessoGrupoFuncionalidade(usuarioLogado.Cpf, FuncionalidadeSistemaEnum.VISUALIZAR_INDICACOES);

            try
            {
                var result = await _sRSRepository.BuscarIndicacaoPremiadaPorId(id, usuarioLogado.OrgId);
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<bool> ValidaAcessoGrupoFuncionalidade(string cpfRequest, FuncionalidadeSistemaEnum enumFuncionalidadeSistema)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            return _usuarioClient.ValidaAcessoGrupoFuncionalidade(cpfRequest, usuarioLogado.Token, enumFuncionalidadeSistema);
        }

        public async Task<ValidarUsuarioLinkedinResponse> ValidarUsuarioLinkedinService(int? userId, string? userName)
        {
            var user = await _srsInfra.ValidarUsuarioRepository(userId, userName);

            if (user == null)
                return new ValidarUsuarioLinkedinResponse { Mensagem = "usuário inválido ou não tem permissão" };

            return new ValidarUsuarioLinkedinResponse
            {
                Mensagem = "Usuário encontrado e valido",
                UserId = user.user_id.ToString()
            };
        }

        public async Task<List<JobOrderDTO>> BuscarVagasDoUltimoAno(string token)
        {
            UsuarioColaboradorDTO user;

            try
            {
                user = await _usuarioClient.ShowMe(token);
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException(e.Message);
            }

            var userId = _srsInfra.BuscaUserId(user.Email);

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Usuário para criação de vaga não encontrado. Procure a área de Recrutamento & Seleção.");
            }

            return await _srsInfra.BuscarVagasDoUltimoAno();
        }

    }
}