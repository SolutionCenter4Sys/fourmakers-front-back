using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using Core.Domain.Usuario;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Linkedin;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Util;
using Aws.Infra.Interfaces;
using Colaboracao.Core.Interfaces;
using ApiClient.Domain;
using System.Text;
using DataTransferObject.Domain.Lote;
using System.Collections.Generic;
using DataTransferObject.Domain.Log;
using Colaborador.Domain.Interfaces.Validadores;
using DocumentFormat.OpenXml.Spreadsheet;
using DataTransferObject.Domain.Org;
using Core.Domain;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Vaga.Enums;
using Microsoft.Extensions.DependencyInjection;
using DocumentFormat.OpenXml.Wordprocessing;

using Logs.Infra.Attributes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ImportacaoColaboradorService : IImportacaoColaboradorService
    {
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly ICurriculoClient _curriculoClient;
        private readonly ILinkedinService _linkedinService;
        private readonly ILogCore _log;
        private readonly IBancoDeTalentosService _bancoDeTalentosService;
        private readonly IColaboradorService _colaboradorService;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IProcessamentoCurriculoLoteRepository _processamentoCurriculoLoteRepository;
        private readonly IQueueProducer _queueProducer;
        private readonly IImportacaoCurriculoValidatorService _validatorService;
        private readonly IUploadFiles _uploadFiles;
        private readonly ILogBancoTalentoSRSRepository _logBancoTalentoSRSRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly string _queueUrl;
        private readonly string _queueLinkedinUrl;
        private readonly IClassificacaoService _classificacaoService;

        public ImportacaoColaboradorService([FromKeyedServices("Dapper")]IUsuarioColaboradorRepository usuarioColaboradorRepository, ICurriculoClient curriculoClient, ILinkedinService linkedinService, ILogCore log, IBancoDeTalentosService bancoDeTalentosService, IColaboradorService colaboradorService, IProcessamentoCurriculoLoteRepository processamentoCurriculoLoteRepository, IQueueProducer queueProducer, IImportacaoCurriculoValidatorService validatorService, IBuscaColaboradorRepository buscaColaboradorRepository, ILogBancoTalentoSRSRepository logBancoTalentoSRSRepository, IVagaFourmakersRepository vagaFourmakersRepository, ICandidaturaRepository candidaturaRepository, IUploadFiles uploadFiles, IClassificacaoService classificacaoService)
        {
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _curriculoClient = curriculoClient;
            _linkedinService = linkedinService;
            _log = log;
            _bancoDeTalentosService = bancoDeTalentosService;
            _colaboradorService = colaboradorService;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _processamentoCurriculoLoteRepository = processamentoCurriculoLoteRepository;
            _queueProducer = queueProducer;
            _queueUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_SQS_QUEUE_BATH_CURRICULO);
            _queueLinkedinUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_SQS_QUEUE_BATH_LINKEDIN);
            _validatorService = validatorService;
            _logBancoTalentoSRSRepository = logBancoTalentoSRSRepository;
            _candidaturaRepository = candidaturaRepository;
            _uploadFiles = uploadFiles;
            _classificacaoService = classificacaoService;
        }

        public async Task<string> ImportarColaborador(byte[] documento, int orgId, string codigoInternoColaboradorCadastrante, bool importacaoLote = false)
        {
            RootIA ret;
            try
            {
                _log.Log($"ImportarColaborador - codigoInternoColaboradorCadastrante - {codigoInternoColaboradorCadastrante}", DataTransferObject.Domain.Log.LevelsEnum.Information);
                ret = await _curriculoClient.GetProfileByDocumentContent(documento, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao ler cv via IA - codigoInternoColaboradorCadastrante - {codigoInternoColaboradorCadastrante}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ValidationException("Não foi possivel ler o documento, ajuste e importe novamente");
            }

            if(String.IsNullOrEmpty(ret.profile.firstName) || String.IsNullOrEmpty(ret.profile.email))
            {
                _log.Log($"Nossa IA nao conseguiu identificar o nome do candidato, pode por favor trocar o CV? - codigo_interno_colaborador - {codigoInternoColaboradorCadastrante}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ApplicationException("Nossa IA nao conseguiu identificar o nome ou o email do candidato, pode por favor trocar o CV?");
            }

            if (await _colaboradorService.ExisteColaboradorComEsteEmail(ret.profile.email) || (await _usuarioColaboradorRepository.BuscarUsuariosOrgPorEmail(ret.profile.email)).Any())
            {
                _log.Log($"Email em uso - codigoInternoColaboradorCadastrante - {codigoInternoColaboradorCadastrante}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ApplicationException("Email em uso");
            }

            if (await _colaboradorService.ExisteColaboradorComEsteLinkedin(ret.UrlLinkedin))
            {
                _log.Log($"Linkedin ja cadastrado - codigoInternoColaboradorCadastrante - {codigoInternoColaboradorCadastrante}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ApplicationException("Linkedin ja cadastrado");
            }

            var colaborador = new ColaboradorDTO();
            colaborador.EmailAlternativo = ret.profile.email;
            colaborador.ContatoPrincipal = ret.profile.phone;
            colaborador.Cpf = Guid.NewGuid().ToString();
            colaborador.NomeCompleto = ret.profile.firstName + " " + ret.profile.lastName;
            
            // Inserir dados do RootIA na nova tabela
            var colaboradorIA = new ColaboradorIADTO
            {
                CodigoInternoColaborador = colaborador.Cpf,
                Email = ret.profile.email,
                Phone = ret.profile.phone,
                FirstName = ret.profile.firstName,
                LastName = ret.profile.lastName,
                Headline = ret.profile.headline,
                Summary = ret.profile.summary,
                LocationName = ret.profile.locationName,
                IndustryName = ret.profile.industryName,
                UrlLinkedin = ret.UrlLinkedin,
                HardSkills = ret.habilidades?.hardSkills != null ? string.Join(",", ret.habilidades.hardSkills) : null,
                SoftSkills = ret.habilidades?.softSkills != null ? string.Join(",", ret.habilidades.softSkills) : null,
                DominioNegocios = ret.habilidades?.dominioNegocios != null ? string.Join(",", ret.habilidades.dominioNegocios) : null,
                Metodologias = ret.habilidades?.metodologias != null ? string.Join(",", ret.habilidades.metodologias) : null
            };
            
            _colaboradorService.InserirDadosIA(colaboradorIA);
            
            _usuarioColaboradorRepository.InsertColaboradorSemUsuario(colaborador);

            try
            {
                var experienciasEmpresaVazia = ret.profile.experience.Where(m => String.IsNullOrEmpty(m.description));

                if(experienciasEmpresaVazia.Any())
                    foreach (var empresa in experienciasEmpresaVazia)
                        empresa.description = "Alterar";

                await _linkedinService.CadastrarSkillsComRetornoIA(colaborador.Cpf, ret, importacaoLote);
            }
            catch (Exception ex)
            {
                var log = $"Houve um erro ao cadastrar as skills para o Colaborador {colaborador.NomeCompleto}, codColaborador: {colaborador.Cpf} - {JsonConvert.SerializeObject(ex)}";
                _log.Log(log, LevelsEnum.Error);
            }

            await _bancoDeTalentosService.InserirBancoDeTalentosFourmakers(colaborador.Cpf, orgId, importacaoLote ? TipoCadastroBancoDeTalentos.FOURMAKERS_LOTE.ToString() : TipoCadastroBancoDeTalentos.FOURMAKERS_UNITARIO.ToString(), codigoInternoColaboradorCadastrante, DateTime.UtcNow, FormaCadastroBancoDeTalentos.CV.ToInt());

            return colaborador.Cpf;
        }

        public async Task<string> ImportarColaboradorLinkedin(string perfilIN, int orgId, string codigoInternoColaboradorCadastrante)
        {
            await _validatorService.ValidaImportarColaboradorLinkedin(perfilIN, orgId, codigoInternoColaboradorCadastrante);

            if (await _colaboradorService.ExisteColaboradorComEsteLinkedin(perfilIN))
                throw new ApplicationException("Linkedin ja cadastrado");

            Root ret;
            try
            {
                ret = await _curriculoClient.GetPerfilLinkedinRapidAPI(perfilIN, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao ler IN via IA", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ValidationException("Perfil inválido");
            }

            if (String.IsNullOrEmpty(ret.profile.firstName))
                throw new ValidationException("Nossa IA nao conseguiu identificar o nome do candidato no Linkedin");

            if (ret.profile == null || ret.profile.entityUrn == null)
                throw new ValidationException("Perfil do linkedin não encontrado");
            
            ColaboradorDTO colaborador = await CadastrarColaboradorSkillsEBancoDeTalentos(perfilIN, orgId, codigoInternoColaboradorCadastrante, ret);

            return colaborador.Cpf;
        }

        private async Task<ColaboradorDTO> CadastrarColaboradorSkillsEBancoDeTalentos(string perfilIN, int orgId, string codigoInternoColaboradorCadastrante, Root ret)
        {
            var colaborador = new ColaboradorDTO();
            colaborador.Cpf = Guid.NewGuid().ToString();
            colaborador.NomeCompleto = ret.profile.firstName + " " + ret.profile.lastName;
            colaborador.UrlLinkedin = perfilIN;
            _usuarioColaboradorRepository.InsertColaboradorSemUsuario(colaborador);

            await _linkedinService.CadastrarSkillsComRetornoIA(colaborador.Cpf, ret);

            await _bancoDeTalentosService.InserirBancoDeTalentosFourmakers(colaborador.Cpf, orgId, TipoCadastroBancoDeTalentos.FOURMAKERS_UNITARIO.ToString(), codigoInternoColaboradorCadastrante, DateTime.UtcNow, FormaCadastroBancoDeTalentos.LINKDEIN.ToInt());
            return colaborador;
        }

        public async Task<ColaboradorDTO> SincronizarColaboradorBancoTalentos(string  perfilIn, int orgId, string token, string email = null, string telefone = null, string codVaga = null)
        {
            if(token != VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("TOKEN_SISTEMA_COLABORACAO"))
            {
                throw new ValidationException("Não autorizado para sincronizar colaborador");
            }
            var codigoInternoColaboradorCadastrante = ClientConfig.Clients.Colaborador.CpfAdmin;
            
            Root ret;
            try
            {
                ret = await _curriculoClient.GetPerfilLinkedinRapidAPI(perfilIn, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao ler IN via IA", LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), LevelsEnum.Error);
                throw new ValidationException("Perfil inválido");
            }

            if(ret.profile == null || ret.profile.entityUrn == null || ret.profile.entityUrn == "")
            {
                throw new ValidationException("Perfil do linkedin não encontrado");
            }

            var codigoInternoColaborador = await _buscaColaboradorRepository.GetCodigoInternoColaboradorByLinkedin(perfilIn);
            if (codigoInternoColaborador == null)
            {
                var colaborador = new ColaboradorDTO();
                codigoInternoColaborador = Guid.NewGuid().ToString();
                colaborador.Cpf = codigoInternoColaborador;
                colaborador.NomeCompleto = ret.profile.firstName + " " + ret.profile.lastName;
                colaborador.UrlLinkedin = perfilIn;
                colaborador.EmailAlternativo = email;
                colaborador.ContatoPrincipal = telefone;
                _usuarioColaboradorRepository.InsertColaboradorSemUsuario(colaborador);
                _usuarioColaboradorRepository.UpsertColaboradorOrg(colaborador, new ColaboradorOrgDTO {
                    Cargo = "-",
                    CodColaborador = colaborador.Cpf,
                    CodDepartamento = "-",
                    CodDiretoria = "BANCO TALENTOS",
                    CodCargo = "-",
                    Departamento = "-",
                    Diretoria = "BANCO TALENTOS",
                    Cpf = colaborador.Cpf,
                    OrgId = orgId,
                    DataAdmissao = DateTime.Now,
                    Ativo = true,
                    ModeloContratacao = "-",
                    EmpresaRelacionada = "-",
                    ModeloTrabalho = "-",
                    DiasPorSemana = 0,
                    ValorHora = 0,
                    CustoHora = 0,
                    BaseHoraMes = 0
                });

                await _linkedinService.CadastrarSkillsComRetornoIA(colaborador.Cpf, ret);

                await _bancoDeTalentosService.InserirBancoDeTalentosFourmakers(colaborador.Cpf, orgId, TipoCadastroBancoDeTalentos.SRS_LINKEDIN.ToString(), codigoInternoColaboradorCadastrante, DateTime.UtcNow, FormaCadastroBancoDeTalentos.LINKDEIN.ToInt());
                if(!String.IsNullOrEmpty(codVaga))
                {
                    try {
                        await CandidatarColaborador(codigoInternoColaborador, codVaga, orgId);
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ao candidatar colaborador: {ex.Message}", LevelsEnum.Error);
                    }
                }
                await _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(codigoInternoColaborador);
                return colaborador;
            }
            else
            {
                if(telefone != null || email != null)
                {
                    _usuarioColaboradorRepository.AtualizarContatoEEmailAlternativo(codigoInternoColaborador, telefone, email);
                }
                await _linkedinService.CadastrarSkillsComRetornoIA(codigoInternoColaborador, ret);
                try {
                    await _bancoDeTalentosService.InserirBancoDeTalentosFourmakers(codigoInternoColaborador, orgId, TipoCadastroBancoDeTalentos.SRS_LINKEDIN.ToString(), codigoInternoColaboradorCadastrante, DateTime.UtcNow, FormaCadastroBancoDeTalentos.LINKDEIN.ToInt());
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro ao inserir banco de talentos: {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                }
                if(!String.IsNullOrEmpty(codVaga))
                {
                    try {
                        await CandidatarColaborador(codigoInternoColaborador, codVaga, orgId);
                    }
                    catch (Exception ex)
                    {
                        _log.Log($"Erro ao candidatar colaborador: {ex.Message}", LevelsEnum.Error);
                    }
                }
                await _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(codigoInternoColaborador);
                return null;
            }

            
        }

        private async Task CandidatarColaborador(string codigoColaborador, string codVaga, int orgId)
        {

            if (await _candidaturaRepository.EstaCandidaturaExiste(codigoColaborador, codVaga))
                throw new ApplicationException("Ja existe uma candidatura desta pessoa para esta vaga.");

            await _candidaturaRepository.CandidatarOutraPessoa(
                codigoColaborador,
                codVaga,
                orgId,
                StatusCandidaturaRecrutamento.InscricaoRegistrada.ToInt(),
                ClientConfig.Clients.Colaborador.CpfAdmin,
                null,
                null,
                null,
                null);
        }

        public async Task<ApiGenericResult<ProcessamentoZipResult>> ImportarColaboradorLote(IFormFile file, int orgId, string codColaborador)
        {
            var result = new ApiGenericResult<ProcessamentoZipResult>();
            var processamentoResult = new ProcessamentoZipResult();

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    processamentoResult.TotalArquivos = archive.Entries.Count;

                    var idLote = Guid.NewGuid();
                    processamentoResult.IdLote = idLote.ToString();
                    _processamentoCurriculoLoteRepository.Cadastrar(idLote, orgId, DateTime.UtcNow, processamentoResult.TotalArquivos, codColaborador, IdentificadorFilaLote.FILA_CURRICULO_ARQUIVO.ToString());

                    foreach (var entry in archive.Entries)
                    {
                        // Só processa arquivos do primeiro nível (ignora subpastas)
                        if (entry.FullName.TrimEnd('/').Contains("/"))
                        {
                            processamentoResult.TotalArquivos = processamentoResult.TotalArquivos - 1;
                            continue;
                        }

                        var pdfResult = new ProcessamentoPdfResult
                        {
                            NomeArquivo = entry.Name
                        };

                        try
                        {
                            // Verificar se é um arquivo PDF
                            if (!entry.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                pdfResult.AProcessar = false;
                                pdfResult.Erro = "Arquivo não é um PDF válido.";
                                processamentoResult.ArquivosComErro++;
                                processamentoResult.PdfsProcessados.Add(pdfResult);
                                continue;
                            }

                            // Extrair o arquivo PDF
                            using (var entryStream = entry.Open())
                            using (var pdfMemoryStream = new MemoryStream())
                            {
                                await entryStream.CopyToAsync(pdfMemoryStream);
                                var pdfBytes = pdfMemoryStream.ToArray();

                                var headers = new System.Collections.Generic.Dictionary<string, string>();
                                headers.Add("idLote", idLote.ToString());

                                // Sempre enviar conteúdo para S3
                                var nomeArquivoSemExtensao = Path.GetFileNameWithoutExtension(pdfResult.NomeArquivo);
                                var s3Key = $"curriculo-colaborador/{nomeArquivoSemExtensao}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                                
                                _log.Log($"Enviando conteúdo para S3: {s3Key}", LevelsEnum.Information);
                                
                                using (var stream = new MemoryStream(pdfBytes))
                                {
                                    var uploadSuccess = await _uploadFiles.UploadFile(stream, s3Key);
                                    if (!uploadSuccess)
                                    {
                                        _log.Log($"Erro ao enviar conteúdo para S3: {s3Key}", LevelsEnum.Error);
                                        throw new Exception($"Erro ao enviar conteúdo para S3: {s3Key}");
                                    }
                                }

                                var mensagemFila = new DataTransferObject.Domain.Colaborador.ImportacaoColaboradorFilaDTO {
                                    IdLote = idLote.ToString(),
                                    NomeArquivo = pdfResult.NomeArquivo,
                                    S3Key = s3Key
                                };
                                
                                var body = JsonConvert.SerializeObject(mensagemFila);
                                
                                _log.Log($"Conteúdo enviado para S3 com sucesso: {s3Key}", LevelsEnum.Information);

                                _log.Log($"Enviando msg {pdfResult.NomeArquivo} pra fila {_queueUrl}", LevelsEnum.Information);

                                if (_queueProducer.Produce(_queueUrl, body, headers))
                                {
                                    pdfResult.AProcessar = true;
                                    processamentoResult.ArquivosAProcessar++;
                                }
                                else
                                {
                                    pdfResult.AProcessar = false;
                                    processamentoResult.ArquivosComErro++;
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            pdfResult.AProcessar = false;
                            pdfResult.Erro = $"Erro ao processar arquivo: {ex.Message}";
                            processamentoResult.ArquivosComErro++;
                        }

                        processamentoResult.PdfsProcessados.Add(pdfResult);
                    }

                    _processamentoCurriculoLoteRepository.AtualizarQuantidadeDeArquivosAProcessar(idLote, processamentoResult.ArquivosAProcessar);
                }
            }

            result.Sucesso = true;
            result.Mensagem = $"Processamento concluído. {processamentoResult.ArquivosAProcessar} arquivos a processar, e {processamentoResult.ArquivosComErro} com erro.";
            result.Retorno = processamentoResult;

            return result;
        }

        public async Task<ApiGenericResult<ProcessamentoPlanilhaResult>> ImportarColaboradorLoteLinkedin(List<string> strings, int orgId, string codColaborador)
        {
            var result = new ApiGenericResult<ProcessamentoPlanilhaResult>();
            var processamentoResult = new ProcessamentoPlanilhaResult();

            var mensagensEnviadas = 0;
            var mensagensComErro = 0;
            var idLote = Guid.NewGuid();
            processamentoResult.IdLote = idLote.ToString();
            processamentoResult.TotalLinhas = strings.Count();

            _processamentoCurriculoLoteRepository.Cadastrar(idLote, orgId, DateTime.UtcNow, processamentoResult.TotalLinhas, codColaborador, IdentificadorFilaLote.FILA_CURRICULO_LINKEDIN.ToString());

            foreach (var str in strings)
            {
                try
                {
                    var messageAttributes = new Dictionary<string, string>
                        {
                            { "idLote", idLote.ToString() }
                        };

                    _log.Log($"Enviando msg {str} pra fila {_queueLinkedinUrl}", LevelsEnum.Information);

                    if (_queueProducer.Produce(_queueLinkedinUrl, str, messageAttributes))
                    {
                        processamentoResult.LinhasAProcessar = processamentoResult.LinhasAProcessar + 1;
                        mensagensEnviadas++;
                    }
                    else
                    {
                        processamentoResult.LinhasComErro = processamentoResult.LinhasComErro + 1;
                        mensagensComErro++;
                    }
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro ao enviar string para SQS: {str}", LevelsEnum.Error);
                    _log.Log(ex.Message, LevelsEnum.Error);
                    processamentoResult.LinhasComErro = processamentoResult.LinhasComErro + 1;
                    mensagensComErro++;
                }
            }

            _processamentoCurriculoLoteRepository.AtualizarQuantidadeDeArquivosAProcessar(idLote, processamentoResult.LinhasAProcessar);

            result.Sucesso = true;
            result.Mensagem = $"Processamento concluído. {mensagensEnviadas} mensagens enviadas com sucesso, {mensagensComErro} com erro.";
            result.Retorno = processamentoResult;

            return result;
        }

        public async Task<string> ImportarColaboradorLinkedinLote(string body, int orgId, string colaboradorCadastrante)
        {
            if (await _colaboradorService.ExisteColaboradorComEsteLinkedin(body))
                throw new ApplicationException($"Linkedin ja cadastrado {body}");

            Root ret;
            try
            {
                ret = await _curriculoClient.GetPerfilLinkedinRapidAPI(body, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("CURRICULO_API_TOKEN"));
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao ler IN via IA", DataTransferObject.Domain.Log.LevelsEnum.Error);
                _log.Log(JsonConvert.SerializeObject(ex), DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ValidationException("Perfil inválido");
            }

            if (ret.profile == null || ret.profile.entityUrn == null)
                throw new ValidationException("Perfil do linkedin não encontrado");

            ColaboradorDTO colaborador = await CadastrarColaboradorSkillsEBancoDeTalentos(body, orgId, colaboradorCadastrante, ret);
            await _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaboradorAsync(colaborador.Cpf);

            return colaborador.Cpf;
        }

        public async Task RegistrarLogBancoTalentoSRSAsync(string urlLinkedin, bool sucesso, string mensagemRetorno = null, string stackTrace = null)
        {
            try
            {
                var logDto = new LogBancoTalentoSRSDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    UrlLinkedin = urlLinkedin,
                    Sucesso = sucesso,
                    MensagemRetorno = mensagemRetorno,
                    StackTrace = stackTrace,
                    Data = DateTime.UtcNow
                };

                await _logBancoTalentoSRSRepository.InserirLogBancoTalentoSRSAsync(logDto);
            }
            catch (Exception ex)
            {
                // Log o erro de log para não quebrar o fluxo principal
                _log.Log($"Erro ao registrar log do Banco de Talentos SRS: {ex.Message}", LevelsEnum.Error);
            }
        }
    }
}