using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using Core.DomainModel.Competencia;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util.Competencia;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class CompetenciaService : ICompetenciaService
    {
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IAspNetUser _aspNetUser;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompetenciaDtoRepository _repositoryCompetencia;
        private readonly IFoursysClient _foursysClient;
        private readonly ITokens _token;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly ICompetenciaColaboradorRepository _competenciaColaboradorRepository;
        private readonly IDBConnectionUnitOfWork _dapperConnectionUnitOfWork;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IPerfilAlocacaoRepository _perfilAlocacaoRepository;
        private readonly ICompetenciaHistoricoService _competenciaHistoricoService;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly IGestaoDeCompetenciaRepository _gestaoDeCompetenciaRepository;
        private readonly ISRSClient _sRSClient;
        private readonly IClassificacaoService _classificacaoService;

        public CompetenciaService(IColaboradorClient colaboradorClient,
                                  IFirebaseClient firebaseClient,
                                  IAspNetUser aspNetUser,
                                  IUploadFilesClient uploadFilesClient,
                                  IConfiguration configuration,
                                  IUnitOfWork unitOfWork,
                                  ICompetenciaDtoRepository repositoryCompetencia,
                                  IFoursysClient foursysClient,
                                  ITokens token,
                                  IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                  ICompetenciaColaboradorRepository competenciaColaboradorRepository,
                                  IDBConnectionUnitOfWork dapperConnectionUnitOfWork,
                                  IHistoricoCVRepository historicoCvRepository,
                                  IPerfilAlocacaoRepository perfilAlocacaoRepository,
                                  ICompetenciaHistoricoService competenciaHistoricoService,
                                  IGestaoDeCompetenciaRepository gestaoDeCompetenciaRepository,
                                  ISRSClient sRsClient, IClassificacaoService classificacaoService)
        {
            _colaboradorClient = colaboradorClient;
            _firebaseClient = firebaseClient;
            _aspNetUser = aspNetUser;
            _uploadFilesClient = uploadFilesClient;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _repositoryCompetencia = repositoryCompetencia;
            _foursysClient = foursysClient;
            _token = token;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _usuarioLogado = _aspNetUser.GetUsuarioLogado();
            _competenciaColaboradorRepository = competenciaColaboradorRepository;
            _dapperConnectionUnitOfWork = dapperConnectionUnitOfWork;
            _historicoCvRepository = historicoCvRepository;
            _perfilAlocacaoRepository = perfilAlocacaoRepository;
            _competenciaHistoricoService = competenciaHistoricoService;
            _gestaoDeCompetenciaRepository = gestaoDeCompetenciaRepository;
            _sRSClient = sRsClient;
            _classificacaoService = classificacaoService;
        }

        public CompetenciaDTO AddCompetencia(string descricao)
        {
            try
            {
                var idUser = _repositoryCompetencia.GetUsuarioCriacaoIdByCpf(_usuarioLogado.Cpf);
                var competenciaId = _repositoryCompetencia.SaveCompetencia(descricao, idUser);
                return new CompetenciaDTO
                {
                    Id = competenciaId,
                    Descricao = descricao,
                    UsuarioCriacaoId = idUser
                };
            }
            catch (ValidationException)
            {
                throw;
            }
        }

        public List<CompetenciaDTO> ListCompetencia(string busca, int cursor, int limite)
        {
            return _repositoryCompetencia.ListCompetencias(busca, cursor, limite);
        }

        public CompetenciaDTO GetCompetenciaById(long id)
        {
            return _repositoryCompetencia.GetCompetenciaById(id);
        }

        public async Task<List<CompetenciaColaboradorDTO>> ListCompetenciaColaborador(string cpfColaborador)
        {
            try
            {
                var listModel = _competenciaColaboradorRepository.GetCompetenciaColaborador(cpfColaborador);

                if (listModel != null && listModel.Count > 0)
                {
                    return await BuildCompetenciaColaborador(listModel);
                }

                return new List<CompetenciaColaboradorDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CompetenciaColaboradorResult AddCompetenciaColaboradorEmLote(List<AddCompetenciaColabParam> param, string cpf)
        {
            var ret = new CompetenciaColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();
            foreach (var item in param)
            {
                try
                {
                    if (item.Id.ToIntOuZero() == 0)
                    {
                        var competenciaAdicionada = AddCompetencia(item.Descricao);
                        item.Id = competenciaAdicionada.Id;
                    }
                    else
                    {
                        var competenciaValidacao = GetCompetenciaById(item.Id);
                        if (competenciaValidacao.Descricao != item.Descricao)
                        {
                            throw new ArgumentException($"Erro: A competência com ID {item.Id} já existe. Descrição: {competenciaValidacao.Descricao}");
                        }
                    }

                    var retAdd = AddCompetenciaColaborador(item.Id, item.NivelId, cpf);

                    ret.Respostas.Add(new ItemPerfilResult
                    {
                        Sucesso = true,
                        ItemId = retAdd.IdCompetencia
                    });
                }
                catch (Exception e)
                {
                    ret.Respostas.Add(new ItemPerfilResult
                    {
                        Sucesso = false,
                        ItemId = item.Id,
                        Mensagem = e.Message
                    });
                }
            }
            ret.Sucesso = true;

            return ret;
        }

        public CompetenciaColaboradorDTO AddCompetenciaColaborador(long CompetenciaId, long? nivelId, string cpf)
        {
            try
            {
                var competenciaColaboradorId = _repositoryCompetencia.SaveCompetenciaColaborador(CompetenciaId, nivelId, cpf);

                var competencia = _repositoryCompetencia.GetCompetenciaById(CompetenciaId);
                var dto = new CompetenciaColaboradorDTO
                {
                    Id = competenciaColaboradorId,
                    IdCompetencia = CompetenciaId,
                    ColaboradorCpf = cpf,
                    IdNivel = nivelId,
                    Competencia = competencia != null ? new CompetenciaDTO
                    {
                        Id = competencia.Id,
                        Descricao = competencia.Descricao,
                    } : null
                };

                _colaboradorClient.EnviaPushNotificationRedeColaborador(cpf,
                    "Nova Competência", " adicionou a Competência" + (competencia?.Descricao ?? ""),
                    _usuarioLogado.Token);
                _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
                return dto;
            }
            catch (ValidationException)
            {
                throw;
            }
        }

        public CompetenciaColaboradorDTO RemoveCompetenciaColaborador(long CompetenciaId, string cpf)
        {
            _repositoryCompetencia.RemoveCompetenciaColaborador(cpf, CompetenciaId);
            _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
            return new CompetenciaColaboradorDTO
            {
                ColaboradorCpf = cpf,
                IdCompetencia = CompetenciaId
            };
        }

        public List<NivelDTO> ListaNivelCompetencia()
        {
            return _repositoryCompetencia.ListNiveisCompetencia();
        }

        private async Task<List<CompetenciaColaboradorDTO>> BuildCompetenciaColaborador(List<CompetenciaColaboradorDTO> listModel)
        {
            var ret = new List<CompetenciaColaboradorDTO>();
            try
            {
                foreach (var item in listModel)
                {
                    var competencia = _repositoryCompetencia.GetCompetenciaById(item.IdCompetencia);
                    if (competencia != null)
                    {
                        item.Competencia = new CompetenciaDTO
                        {
                            Id = competencia.Id,
                            UsuarioCriacaoId = competencia.UsuarioCriacaoId,
                            Descricao = competencia.Descricao,
                            Pendente = competencia.Pendente
                        };
                    }

                    item.Nivel = null;
                    if (item.Nivel != null || item.IdNivel > 0)
                    {
                        var nivel = _repositoryCompetencia.GetNivelById(item.IdNivel);
                        if (nivel != null)
                        {
                            item.Nivel = nivel;
                        }
                    }

                    item.Certificados = new List<CertificadoDTO>();
                    if (item.ListaIdCertificado != null)
                    {
                        foreach (var idCertificado in item.ListaIdCertificado)
                        {
                            var certificado = _repositoryCompetencia.GetCertificadoByIdRelacao(idCertificado);
                            if (certificado != null)
                            {
                                item.Certificados.Add(certificado);
                            }
                        }

                        foreach (var caminhoCertificados in item.Certificados)
                        {
                            if (caminhoCertificados.Path != null)
                                caminhoCertificados.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + caminhoCertificados.Path;
                        }
                    }

                    var endosso = _repositoryCompetencia.GetEndossoByCompetenciaColaboradorId(item.Id);
                    item.Endosso = endosso;
                    item.EndossoConcedido = new List<EndossoColaboradorDTO>();

                    if (endosso != null && endosso.Endossado)
                        item.EndossoConcedido = await GetListaEndossoConcedido(item.Id);

                    if (endosso == null || endosso.QuantidadeSolicitacaoEndosso == 0)
                        item.Endosso = null;

                    ret.Add(item);
                }

                return ret.OrderBy(x => x.Competencia?.Descricao).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<EndossoColaboradorDTO>> GetListaEndossoConcedido(long idTbColaboradorCompetencia)
        {
            return new List<EndossoColaboradorDTO>();
        }

        public void RemoveCertificadoCompetenciaColaborador(long idCertificadoCompetencia, string cpfColaborador)
        {
            var certificadoColab = _repositoryCompetencia.GetColaboradorCompetenciaCertificado(idCertificadoCompetencia);

            if (certificadoColab == null)
                throw new Exception("Certificado não encontrado.");
            if (certificadoColab.Ativo == 0)
                throw new Exception("Certificado já foi removido.");
            if (certificadoColab.CpfTbColaboradroCompetencia != cpfColaborador)
                throw new Exception("Este certificado não pertence ao colaborador.");

            var ultimoCertificado = _repositoryCompetencia.BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(idCertificadoCompetencia);

            if (certificadoColab.Principal == 1 && ultimoCertificado != null && ultimoCertificado.Id > 0)
            {
                _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(ultimoCertificado.Id, ultimoCertificado.Ativo, 1);
            }
            _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(idCertificadoCompetencia, 0, certificadoColab.Principal);
        }

        public void AlteraCertificadoPrincipalColaborador(long idCertificadoCompetencia, string cpfColaborador)
        {
            var certificadoColab = _repositoryCompetencia.GetColaboradorCompetenciaCertificado(idCertificadoCompetencia);

            if (certificadoColab == null)
                throw new Exception("Certificado não encontrado.");
            if (certificadoColab.Ativo == 0)
                throw new Exception("Certificado já foi removido.");
            if (certificadoColab.CpfTbColaboradroCompetencia != cpfColaborador)
                throw new Exception("Este certificado não pertence ao colaborador.");

            var antigoPrincipal = _repositoryCompetencia.BuscarColaboradorCompetenciaCertificadoPrincipal(idCertificadoCompetencia);
            _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(antigoPrincipal.Id, antigoPrincipal.Ativo, 0);
            _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(idCertificadoCompetencia, certificadoColab.Ativo, 1);
        }

        public CompetenciaColaboradorDTO AlterarCompetenciaColaborador(string cpf, long id, long? idNivel)
        {
            try
            {
                _repositoryCompetencia.AtualizaCompetenciaColaborador(cpf, id, idNivel);

                var competencia = _repositoryCompetencia.GetCompetenciaById(id);
                var dto = new CompetenciaColaboradorDTO
                {
                    ColaboradorCpf = cpf,
                    IdCompetencia = id,
                    IdNivel = idNivel,
                    Competencia = competencia != null ? new CompetenciaDTO
                    {
                        Id = competencia.Id,
                        Descricao = competencia.Descricao
                    } : null
                };

                _colaboradorClient.EnviaPushNotificationRedeColaborador(cpf,
                    "Competência atualizada!", " atualizou a competência " + (competencia?.Descricao ?? ""),
                    _usuarioLogado.Token);
                _classificacaoService.AtualizarClassificacaoProfissionalPorCodigoInternoColaborador(cpf);
                return dto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> ListarIdsPorCompetenciaId(long id)
        {
            return _repositoryCompetencia.ListarIdsPorCompetenciaId(id);
        }

        public async Task<CertificadoDTO> AlteraCertificado(string cpfRequest, long certificadoId, byte[] certificado, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var certificadoDto = _repositoryCompetencia.GetCertificadoById(certificadoId);

                    if (certificadoDto == null || certificadoDto.IdCertificado == null)
                    {
                        throw new Exception("Certificado nao encontrado!");
                    }

                    if (certificado != null)
                    {
                        var nomeArquivo = cpfRequest + "_COMPETENCIA_" + DateTime.Now.ToString("yyyyMMddHHmmssF");

                        if (tipo == TipoCertificadoEnum.IMAGEM)
                            certificadoDto.Path = nomeArquivo + ".png";
                        else if (tipo == TipoCertificadoEnum.PDF)
                            certificadoDto.Path = nomeArquivo + ".pdf";

                        var pathPastaCertificadoS3 = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + certificadoDto.Path;

                        if (tipo == TipoCertificadoEnum.IMAGEM)
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, certificado);
                        }
                        else if (tipo == TipoCertificadoEnum.PDF)
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, certificado);
                            var thumb = GeradorThumbUtil.ConverterPDF(certificado, 150);
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3.Replace(".pdf", "_thumb.pdf"), thumb);
                        }

                        certificadoDto.descricao = descricao;
                        certificadoDto.instituicao = instituicao;
                        certificadoDto.conclusao = conclusao;
                        certificadoDto.cargaHoraria = cargaHoraria;
                        _repositoryCompetencia.UpdateCertificado(certificadoDto);
                        certificadoDto.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + pathPastaCertificadoS3;
                    }
                    else
                    {
                        certificadoDto.descricao = descricao;
                        certificadoDto.instituicao = instituicao;
                        certificadoDto.conclusao = conclusao;
                        certificadoDto.cargaHoraria = cargaHoraria;
                        _repositoryCompetencia.UpdateCertificado(certificadoDto);
                    }
                    dbTrans.Commit();
                    _historicoCvRepository.InserirHistoricoCV(cpfRequest, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.CERTIFICACAO);
                    return certificadoDto;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public async Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpfRequest, long? competenciaColaboradorId, byte[] certificado, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    long? competenciaColabRowId = null;
                    if (competenciaColaboradorId != null)
                    {
                        competenciaColabRowId = _repositoryCompetencia.GetCompetenciaColabRowId((long)competenciaColaboradorId);
                        if (competenciaColabRowId == null)
                        {
                            throw new Exception("Só é possível enviar certificados para competências que o colaborador possuí.");
                        }
                    }

                    var certificadoDto = new CertificadoDTO();

                    if (certificado != null)
                    {
                        var nomeArquivo = cpfRequest + "_COMPETENCIA_" + DateTime.Now.ToString("yyyyMMddHHmmssF");

                        if (tipo == TipoCertificadoEnum.IMAGEM)
                            certificadoDto.Path = nomeArquivo + ".png";
                        else
                            certificadoDto.Path = nomeArquivo + ".pdf";
                    }
                    else
                    {
                        certificadoDto.Path = "";
                    }

                    certificadoDto.descricao = descricao;
                    certificadoDto.instituicao = instituicao;
                    certificadoDto.conclusao = conclusao;
                    certificadoDto.cargaHoraria = cargaHoraria;

                    _repositoryCompetencia.SaveCertificado(certificadoDto, cpfRequest, competenciaColabRowId);

                    if (certificadoDto.Path != "")
                    {
                        var pathPastaCertificadoS3 = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + certificadoDto.Path;

                        if (certificado != null)
                        {
                            if (tipo == TipoCertificadoEnum.IMAGEM)
                            {
                                await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, certificado);
                            }
                            else
                            {
                                await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, certificado);
                                var thumb = GeradorThumbUtil.ConverterPDF(certificado, 150);
                                await _uploadFilesClient.UploadFile(pathPastaCertificadoS3.Replace(".pdf", "_thumb.pdf"), thumb);
                            }
                            certificadoDto.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + pathPastaCertificadoS3;
                        }
                    }

                    dbTrans.Commit();
                    _historicoCvRepository.InserirHistoricoCV(cpfRequest, OrigemAlteracaoCVEnum.MANUAL, TipoItemCVEnum.INSERT, null, null, ItemCVEnum.CERTIFICACAO);
                    return certificadoDto;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public void RemoverCertificado(long id, string cpf)
        {
            var certificadoColab = _repositoryCompetencia.GetColaboradorCertificadoByCertificadoId(id);
            var certificado = _repositoryCompetencia.GetCertificadoById(id);

            if (certificadoColab == null)
                throw new Exception("Certificado não encontrado.");
            if (certificadoColab.Ativo == 0)
                throw new Exception("Certificado já foi removido.");
            if (certificadoColab.CpfTbColaboradroCompetencia != cpf)
                throw new Exception("Este certificado não pertence ao colaborador.");

            var ultimoCertificado = _repositoryCompetencia.BuscarUltimoCertificadoParaACompetenciaDesteCertificadoQueEstaSendoExcluido(certificadoColab.Id);

            if (certificadoColab.Principal == 1 && ultimoCertificado != null && ultimoCertificado.Id > 0)
            {
                _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(ultimoCertificado.Id, ultimoCertificado.Ativo, 1);
            }

            if (certificado != null)
            {
                certificado.ativo = false;
                _repositoryCompetencia.UpdateCertificado(certificado);
            }
            _repositoryCompetencia.UpdateColaboradorCompetenciaCertificado(certificadoColab.Id, 0, certificadoColab.Principal);
        }

        public CompetenciasSumarioResult SumarioCompetencias(FiltroSumarioCompetenciasParam param)
        {
            try
            {
                ValidaAcessoSumarioCompetencia();

                if (param.UnidadeId.Count < 1) throw new Exception("Necessário selecionar uma unidade");
                if (param.competencia.Count < 1 && param.Cpf.Count < 1) throw new Exception("Necessário selecionar um colaborador ou skill");
                var token = _token.Base64(_aspNetUser.GetUsuarioLogado().Token);
                var orgId = _aspNetUser.GetUsuarioLogado().OrgId;
                return _repositoryCompetencia.BuscarColaboradorSumario(param, token, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ItensSumarioResult> SumarioCompetenciasListarSkills(int cursor, int limite, string? descricao)
        {
            ValidaAcessoSumarioCompetencia();
            return await _repositoryCompetencia.ListarSkillsSumario(cursor, limite, descricao);
        }

        public List<SkillSumarioDTO> SumarioCompetenciasListarNiveis()
        {
            return _repositoryCompetencia.ListarNiveisSumario();
        }

        private void ValidaAcessoSumarioCompetencia()
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                _usuarioLogado.Cpf, _usuarioLogado.OrgId, FuncionalidadeSistemaEnum.SUMARIO_DE_COMPETENCIA).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para sumário de competência");
            }
        }

        public List<long> ListarCompetenciasAtribuidas(string cpfColaborador)
        {
            return _competenciaColaboradorRepository.ListarCompetenciasAtribuidas(cpfColaborador);
        }

        public List<UnidadesDTO> ListUnidadesPorOrg(string token, int orgId)
        {
            var unidades = _foursysClient.ListarUnidadesPorOrg(token, orgId).Result.ListaUnidades;
            return unidades.OrderBy(x => x.Id).ToList();
        }

        public List<UnidadesDTO> ListUnidadesComDefaultPorOrg(string token, int orgId)
        {
            var unidades = _foursysClient.ListarUnidadesPorOrg(token, orgId).Result.ListaUnidades;
            unidades.Add(new UnidadesDTO
            {
                Descricao = "TODAS",
                Id = "0"
            });
            return unidades.OrderBy(x => x.Id).ToList();
        }

        public List<CompetenciaSugeridaDTO> ListarCompetenciasSugeridas(TipoCompetenciaSRSEnum competencia)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)competencia);

            return _repositoryCompetencia.ListarCompetenciasSugeridas(competencia);
        }

        public Task<List<CompetenciaConsolidadaDTO>> ListarCompetenciasConsolidadas(TipoCompetenciaSRSEnum competencia)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)competencia);

            return _repositoryCompetencia.ListarCompetenciasConsolidadas(competencia);
        }

        private void ValidaAcessoGestaoCompetencias()
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                            _usuarioLogado.Cpf, _usuarioLogado.OrgId, FuncionalidadeSistemaEnum.GESTAO_COMPETENCIAS).Result;
            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para gestão de competências");
            }
        }

        public async Task AprovarCompetencia(int idCompetenciaASerAprovada, TipoCompetenciaSRSEnum tipoCompetencia)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)tipoCompetencia);

            var competencia = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoEId(idCompetenciaASerAprovada, tipoCompetencia);

            await _repositoryCompetencia.AprovarCompetencia(idCompetenciaASerAprovada, tipoCompetencia);

            if (competencia != null)
            {
                _competenciaHistoricoService.InserirHistoricoCompetencia(
                    AcaoHistoricoCompetenciaEnum.Aprovar, tipoCompetencia, competencia.Descricao, competencia.CpfUsuarioCriacao);
            }
        }

        public void ValidaTipoCompetenciaEnum(int enumValue)
        {
            if (!Enum.IsDefined(typeof(TipoCompetenciaSRSEnum), enumValue))
                throw new ArgumentException("Competência inválida.");
        }

        public async Task ReprovarCompetencia(int idCompetenciaASerReprovada, TipoCompetenciaSRSEnum tipoCompetencia)
        {
            var itemPerfilEnum = CompetenciaUtils.ConverterTipoCompetenciaSRSToItemPerfil(tipoCompetencia);
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)tipoCompetencia);
            await _repositoryCompetencia.ReprovarCompetencia(idCompetenciaASerReprovada, tipoCompetencia);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciaColaborador(idCompetenciaASerReprovada, tipoCompetencia);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarGestorExternoPerfil(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarAlocado(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarPerfil(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarVagaFourmakers(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarVaga(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarVagaSRS(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.ReprovarAssociacaoCompetenciarVagaCandidato(idCompetenciaASerReprovada, itemPerfilEnum);
            await _repositoryCompetencia.InsereLogReprovacaoCompetencia(idCompetenciaASerReprovada, tipoCompetencia, _usuarioLogado.Cpf);
        }

        public async Task ValidaSkillSugerida(int idCompetenciaSugerida, TipoCompetenciaSRSEnum tipoCompetencia)
        {
            var competencia = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoEId(idCompetenciaSugerida, tipoCompetencia);

            if (competencia is null)
                throw new ArgumentException("Não foi possivel unificar as competências. Competência Sugerida não encontrada!");
            if (!competencia.Pendente)
                throw new ArgumentException("Não foi possivel unificar as competências. Competência sugerida está confirmada!");
        }

        private async Task ValidaSkillExistente(int idCompetencia, TipoCompetenciaSRSEnum tipoCompetencia)
        {
            var competencia = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoEId(idCompetencia, tipoCompetencia);
            if (competencia is null)
                throw new ArgumentException("Não foi possivel unificar as competências. Competência não encontrada!");
        }

        public async Task ValidaSkillConsolidada(int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipoCompetencia)
        {
            var competencia = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoEId(idCompetenciaConsolidada, tipoCompetencia);
            if (competencia is null)
                throw new ArgumentException("Não foi possivel unificar as competências. Competência Consolidada não encontrada!");
            if (competencia.Pendente)
                throw new ArgumentException("Não foi possivel unificar as competências. Competência Consolidada não está confirmada!");
        }

        public async Task UnificarCompetencia(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipoCompetencia, string cpfRequest, string tokenUsuarioLogado)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)tipoCompetencia);
            await ValidaSkillSugerida(idCompetenciaSugerida, tipoCompetencia);
            await ValidaSkillConsolidada(idCompetenciaConsolidada, tipoCompetencia);

            _dapperConnectionUnitOfWork.BeginTransaction();
            try
            {
                await Unificar(idCompetenciaSugerida, idCompetenciaConsolidada, tipoCompetencia, cpfRequest, tokenUsuarioLogado);
            }
            catch (Exception)
            {
                _dapperConnectionUnitOfWork.Rollback();
                throw;
            }
            _dapperConnectionUnitOfWork.Commit();
        }

        private async Task<AlterarTabelasUnificacaoCuradoriaDTO> Unificar(int idCompetenciaSugerida, int idCompetenciaConsolidada, TipoCompetenciaSRSEnum tipoCompetencia, string cpfRequest, string tokenUsuarioLogado)
        {
            var ids = new long[] { idCompetenciaSugerida, idCompetenciaConsolidada };
            var competencias = await _competenciaColaboradorRepository.BuscarCompetenciaColaboradorPorCompetenciaIds(ids);
            var listaCompetenciasParaExcluir = competencias.GroupBy(c => c.ColaboradorCpf)
                .Where(g => g.Count() > 1)
                .Select(g => g
                    .OrderByDescending(c => c.Nivel != null)
                    .ThenBy(c => c.Nivel?.PrioridadeUnificacao ?? int.MaxValue)
                    .Last())
                .ToList();

            if (listaCompetenciasParaExcluir.Count() > 0)
            {
                foreach (var competencia in listaCompetenciasParaExcluir)
                {
                    await _competenciaColaboradorRepository.ExcluirColaboradorCompetenciaPorId(competencia.Id);
                }
            }

            var linhasAfetadas = await _repositoryCompetencia.AtualizarCompetenciaColaboradorDeSugeridaParaConsolidada(idCompetenciaSugerida, idCompetenciaConsolidada, tipoCompetencia);

            var tipoPerfilEnum = CompetenciaUtils.ConverterTipoCompetenciaSRSToItemPerfil(tipoCompetencia);
            var alocadosSkills = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoAlocadoSkills(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var perfilSkills = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoPerfilSkills(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var vagasSkills = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVaga(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var vagasCandidatosSkill = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagasCandidato(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var vagasFourmakersSkill = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var perfilExternoSkills = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoGestorExternoPerfilSkills(idCompetenciaSugerida, idCompetenciaConsolidada, (int)tipoPerfilEnum);
            var vagasSrsSkills = await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagasSRS(idCompetenciaSugerida, idCompetenciaSugerida, (int)tipoPerfilEnum);
            await _sRSClient.AlterarCategoriaHabilidade(
                new()
                {
                    TipoAntigo = (int)tipoCompetencia,
                    TipoNovo = (int)tipoCompetencia,
                    IdHabilidadeAntiga = idCompetenciaSugerida,
                    IdHabilidadeNova = idCompetenciaConsolidada,
                },
                tokenUsuarioLogado
            );

            await _repositoryCompetencia.DesativarCompetencia(idCompetenciaSugerida, tipoCompetencia);
            await _repositoryCompetencia.GravaLogUnificacao(idCompetenciaSugerida, idCompetenciaConsolidada, tipoCompetencia, cpfRequest);
            var colaboradoresUnificacaoId = competencias.Select(x => x.ColaboradorCpf).ToList();
            await _classificacaoService.AtualizarClassificacaoProfissionalPorListaDeCodigoInternoColaboradorAsync(colaboradoresUnificacaoId);
            return new()
            {
                Alocados = alocadosSkills,
                Perfis = perfilSkills,
                Vagas = vagasSkills,
                VagasCandidato = vagasCandidatosSkill,
                PerfisExterno = perfilExternoSkills,
                VagasSrs = vagasSrsSkills,
                VagasFourmakers = vagasFourmakersSkill,
                Colaboradores = linhasAfetadas
            };
        }

        public Task<List<LogCompetenciaDTO>> ListarLogCompetencias(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)enumTipoCompetencia);
            return _repositoryCompetencia.ListarLogCompetencias(enumTipoCompetencia);
        }

        public async Task<EditarCompetenciaDTO> EditarCompetencia(EditarCompetenciaParam param, bool pulaValidacaoAcesso = false, bool pulaValidacaoDuplicidade = false)
        {
            if (!pulaValidacaoAcesso)
            {
                ValidaAcessoGestaoCompetencias();
            }
            ValidaTipoCompetenciaEnum((int)param.TipoCompetencia);

            var competenciaAnterior = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoEId(param.Id, param.TipoCompetencia);

            var resultado = await _repositoryCompetencia.EditarCompetencia(param, pulaValidacaoDuplicidade);

            if (competenciaAnterior != null && !competenciaAnterior.Descricao.ToUpper().Equals(param.Descricao.ToUpper()))
            {
                await _competenciaHistoricoService.EditarHistoricoCompetencia(
                    AcaoHistoricoCompetenciaEnum.Unificar, param.TipoCompetencia, competenciaAnterior.Descricao, param.Descricao, competenciaAnterior.CpfUsuarioCriacao);
            }

            return resultado;
        }

        public Task<AdicionarCompetenciaDTO> AdicionarCompetencia(string descricao, TipoCompetenciaSRSEnum competencia, string cpf)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)competencia);
            var ret = _repositoryCompetencia.AdicionarCompetencia(descricao, competencia, cpf);
            _competenciaHistoricoService.InserirHistoricoCompetencia(AcaoHistoricoCompetenciaEnum.Adicionar, competencia, descricao, cpf);
            return ret;
        }

        public Task<AdicionarCompetenciaDTO> AdicionarCompetenciaSemTransaction(string descricao, TipoCompetenciaSRSEnum competencia, string cpf, int orgId)
        {
            ValidaAcessoGestaoCompetencias();
            ValidaTipoCompetenciaEnum((int)competencia);
            var ret = _repositoryCompetencia.AdicionarCompetenciaCvGestaoDeSkills(descricao, competencia, cpf, orgId);
            _competenciaHistoricoService.InserirHistoricoCompetencia(AcaoHistoricoCompetenciaEnum.Adicionar, competencia, descricao, cpf);
            return ret;
        }

        public List<PerfilAlocacaoDTO> ListarPerfilAlocacao(int orgId)
        {
            ValidaAcessoSumarioCompetencia();
            return _perfilAlocacaoRepository.ListarPerfilAlocacao(null, orgId);
        }

        public async Task<ApiGenericResult<AlterarNomeCompetenciaResultDTO>> AlterarNomeCompetencia(string nomeAtual, string novoNome, TipoCompetenciaSRSEnum tipo, string cpfReuest, string tokenUsuarioLogado)
        {
            var ApiResult = new ApiGenericResult<AlterarNomeCompetenciaResultDTO>();

            _dapperConnectionUnitOfWork.BeginTransaction();
            try
            {
                var checkSkill = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoENome(nomeAtual, tipo);
                if (checkSkill == null)
                {
                    throw new ArgumentException($"Habilidade com o nome {nomeAtual} Não encontrada");
                }

                var verificaSeJáExisteDestino = await _repositoryCompetencia.ObterCompetenciaAtivaPorTipoENome(novoNome, tipo);
                var alteracoesLog = new AlterarTabelasUnificacaoCuradoriaDTO();
                int idAlterado = 0;
                if (verificaSeJáExisteDestino != null)
                {
                    alteracoesLog = await Unificar((int)checkSkill.Id, (int)verificaSeJáExisteDestino.Id, tipo, cpfReuest, tokenUsuarioLogado);
                    await EditarCompetencia(new()
                    {
                        Id = (int)verificaSeJáExisteDestino.Id,
                        Ativo = true,
                        Descricao = novoNome,
                        TipoCompetencia = tipo
                    }, true, true);
                    idAlterado = (int)verificaSeJáExisteDestino.Id;
                }
                else
                {
                    await EditarCompetencia(new()
                    {
                        Id = (int)checkSkill.Id,
                        Ativo = true,
                        Descricao = novoNome,
                        TipoCompetencia = tipo
                    }, true, true);
                    idAlterado = (int)checkSkill.Id;
                }

                var retorno = new AlterarNomeCompetenciaResultDTO()
                {
                    NomeCompetencia = checkSkill.Descricao,
                    CompetenciaID = (int)checkSkill.Id,
                    NovoNome = novoNome,
                    NovoId = idAlterado == checkSkill.Id ? null : idAlterado,
                    Tipo = idAlterado == checkSkill.Id ? "Renomeado" : "Unificado",
                    LogsAlteracao = alteracoesLog
                };

                ApiResult.Retorno = retorno;
                _dapperConnectionUnitOfWork.Commit();
            }
            catch (Exception e)
            {
                _dapperConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Update, $"Nome da habilidade: {nomeAtual}, para: {novoNome}, StackTrace: {e.Message}");
            }

            return ApiResult;
        }

        public async Task<ApiGenericResult<List<CompetenciaNomeEIdDTO>>> ListarNomeDeSkillsPorTipo(TipoCompetenciaSRSEnum tipo)
        {
            var ApiResult = new ApiGenericResult<List<CompetenciaNomeEIdDTO>>();
            try
            {
                ApiResult.Retorno = await _repositoryCompetencia.ObterNomeSkillsPorTipo(tipo);
            }
            catch (Exception e)
            {
                ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, $"Skills por nome");
            }

            return ApiResult;
        }

        public async Task SincronizarSkillsNaoUnificadasNaCuradoriaAntiga()
        {
            var hardskills = await _gestaoDeCompetenciaRepository.ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum.COMPETENCIA);
            var metodologias = await _gestaoDeCompetenciaRepository.ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum.METODOLOGIA);
            var idiomas = await _gestaoDeCompetenciaRepository.ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum.IDIOMA);
            var dominios = await _gestaoDeCompetenciaRepository.ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum.DOMINIONEGOCIO);
            var softskill = await _gestaoDeCompetenciaRepository.ObterListaSkillNaoSincronizadasNaCuradoriaVagaFourmakersSkillsPorItemPerfilId(ItemPerfilEnum.SOFTSKILL);

            foreach (var item in hardskills)
                await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(item.SkillIdOld, item.SkillIdNew, (int)ItemPerfilEnum.COMPETENCIA);
            foreach (var item in metodologias)
                await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(item.SkillIdOld, item.SkillIdNew, (int)ItemPerfilEnum.METODOLOGIA);
            foreach (var item in idiomas)
                await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(item.SkillIdOld, item.SkillIdNew, (int)ItemPerfilEnum.IDIOMA);
            foreach (var item in dominios)
                await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(item.SkillIdOld, item.SkillIdNew, (int)ItemPerfilEnum.DOMINIONEGOCIO);
            foreach (var item in softskill)
                await _gestaoDeCompetenciaRepository.AlterarIdHabilidadePorTipoSkillsVagaFourmakers(item.SkillIdOld, item.SkillIdNew, (int)ItemPerfilEnum.SOFTSKILL);
        }

        public async Task<ApiGenericResult<List<VwSkillColaboradorDTO>>> ListarSkillsColaborador(string codigoInternoColaborador)
        {
            var apiResult = new ApiGenericResult<List<VwSkillColaboradorDTO>>();
            var skills = await _repositoryCompetencia.BuscarSkillsPorCodigoInternoColaborador(codigoInternoColaborador);
            apiResult.Retorno = skills;
            return apiResult;
        }

        public async Task<ApiGenericResult<SkillsLog>> GravarLogsSkillsMinhaJornada(SkillsLog logSkill)
        {
            var apiResult = new ApiGenericResult<SkillsLog>();

            logSkill.LogAutomatico = false;
            logSkill.CodigoInternoColaboradorLogado = _usuarioLogado.Cpf;

            var skillLog = await _repositoryCompetencia.GravarLogsSkillsMinhaJornada(logSkill);

            apiResult.Retorno = skillLog;
            return apiResult;
        }
    }
}
