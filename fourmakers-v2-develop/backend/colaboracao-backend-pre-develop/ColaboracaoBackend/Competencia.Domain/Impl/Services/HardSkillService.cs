using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.Domain;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Competencia;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class HardSkillService : IHardSkillService
    {
        private readonly IHardSkillRepository _hardSkillRepository;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IFoursysClient _foursysClient;
        private readonly IAspNetUser _aspNetUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICompetenciaColaboradorRepository _competenciaColaboradorRepository;
        private readonly ITokens _token;
        private IHistoricoCVRepository _historicoCvRepository;
        private readonly ICompetenciaDtoRepository _competenciaRepository;
        private readonly ISkillGenericService _skillGenericService;

        public HardSkillService(IFoursysClient foursysClient, ITokens token, IAspNetUser aspNetUser, IHardSkillRepository hardSkillRepository,
            IUploadFilesClient uploadFilesClient, IUnitOfWork unitOfWork, IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            ICompetenciaColaboradorRepository competenciaColaboradorRepository, IHistoricoCVRepository historicoCvRepository, ICompetenciaDtoRepository competenciaRepository, ISkillGenericService skillGenericService)
        {
            _token = token;
            _aspNetUser = aspNetUser;
            _foursysClient = foursysClient;
            _usuarioLogado = _aspNetUser.GetUsuarioLogado();
            _hardSkillRepository = hardSkillRepository;
            _uploadFilesClient = uploadFilesClient;
            _unitOfWork = unitOfWork;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _competenciaColaboradorRepository = competenciaColaboradorRepository;
            _historicoCvRepository = historicoCvRepository;
            _competenciaRepository = competenciaRepository;
            _skillGenericService = skillGenericService;
        }

        public CompetenciaDTO InserirHardSkillColaborador(long competenciaId, long? nivelId, string cpf, bool minhaJornada, string gestorExternoPerfil, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var skillsColaborador = _hardSkillRepository.ListarCodigoCompetenciasColaborador(cpf);
                if (skillsColaborador.Any(x => x == competenciaId))
                {
                    throw new ArgumentException($"Você já possui essa hard skill: {skillsColaborador.Where(x => x == competenciaId).Single()}");
                }
                var ret = _hardSkillRepository.InserirHardSkillColaborador(competenciaId, nivelId, cpf);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, competenciaId, nivelId, ItemCVEnum.HARDSKILL);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = competenciaId,
                        ItemPerfil = ItemPerfilEnum.COMPETENCIA,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ADICIONADO_PERFIL,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = cpf
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CompetenciaDTO InserirHardSkill(string descricao, string cpf)
        {
            try
            {
                var usuarioCriacaoId = _hardSkillRepository.ObterIdUsuarioPorCpf(cpf);
                var listaSkillsComMesmaDescricao = _hardSkillRepository.ListarHardSkillPorDescricao(descricao);

                var skillComMesmaDescricao = listaSkillsComMesmaDescricao.SingleOrDefault();

                if (skillComMesmaDescricao is not null)
                {
                    if (skillComMesmaDescricao.Ativo)
                    {
                        throw new ArgumentException($"Já existe skill com esse nome: {skillComMesmaDescricao.Id} - {skillComMesmaDescricao.Descricao} ");
                    }

                    skillComMesmaDescricao.Ativo = true;
                    skillComMesmaDescricao.Pendente = true;

                    _hardSkillRepository.AtualizarHardSkill(skillComMesmaDescricao);
                }

                return _hardSkillRepository.InserirHardSkill(descricao, usuarioCriacaoId ?? 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CompetenciaColaboradorResult InserirHardSkillColaboradorEmLote(List<AddCompetenciaColabParam> param, string cpf, bool minhaJornada)
        {
            var ret = new CompetenciaColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();
            foreach (var item in param)
            {
                try
                {
                    if (item.Id.ToIntOuZero() == 0)
                    {
                        var competenciaAdicionada = InserirHardSkill(item.Descricao, cpf);
                        item.Id = competenciaAdicionada.Id;
                    }
                    else
                    {
                        var competenciaValidacao = ObterHardSkillPorId(item.Id);
                    }

                    var retAdd = InserirHardSkillColaborador(item.Id, item.NivelId, cpf, minhaJornada, item.GestorExternoPerfil);

                    ret.Respostas.Add(new ItemPerfilResult
                    {
                        Sucesso = true,
                        ItemId = retAdd.Id
                    });
                }
                catch (Exception e)
                {
                    throw;
                    // ret.Respostas.Add(new ItemPerfilResult
                    // {
                    //     Sucesso = false,
                    //     ItemId = item.Id,
                    //     Mensagem = e.Message
                    // });
                }
            }
            ret.Sucesso = true;

            return ret;
        }

        public CompetenciaDTO ObterHardSkillPorId(long competenciaId)
        {
            try
            {
                var hardSkill = _hardSkillRepository.ObterHardSkillPorId(competenciaId);
                if (hardSkill == null)
                {
                    throw new ArgumentException($"Não foi possível encontrar a Hard Skill com Id: {competenciaId}");
                }
                return hardSkill;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ItemPerfilDTO> ListarHardSkill(string busca, int cursor, int limite)
        {
            try
            {
                var competenciasList = _hardSkillRepository.ListarHardSkill(busca, cursor, limite);
                return competenciasList.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.Id,
                    Pendente = x.Pendente
                }).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<List<CompetenciaDTO>> ListarHardSkillColaborador(string cpfColaborador)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiGenericResult<List<CompetenciaGroupDTO>>> GetGroupCompetenciaByIds(List<long> ids)
        {
            var resultado = new ApiGenericResult<List<CompetenciaGroupDTO>>();
            if (ids is null || ids.Count == 0)
                return resultado;

            // int ItemPerfilTipoID
            var encontradas = await _competenciaRepository.GetCompetenciasByTypeAndGroupId((int)ItemPerfilEnum.COMPETENCIA, ids.Distinct().ToList());

            resultado.Retorno = encontradas;
            return resultado;
        }


        public bool RemoverHardSkillColaborador(long competenciaId, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var competenciaColaborador = _hardSkillRepository.ObterCompetenciaColaborador(competenciaId, cpf);
                if (competenciaColaborador == null)
                {
                    throw new ArgumentException("O colaborador não possui essa competência");
                }
                var ret = _hardSkillRepository.RemoverHardSkillColaborador(competenciaColaborador.Id);
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, competenciaId, null, ItemCVEnum.HARDSKILL);
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NivelDTO> ListarNivelHardSkill()
        {
            try
            {
                return _hardSkillRepository.ListarNivelHardSkill();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool RemoverCertificadoHardSkillColaborador(long certificadoCompetenciaId, string cpf)
        {
            try
            {
                return _hardSkillRepository.InativarCertificadoHardSkillColaborador(certificadoCompetenciaId, cpf);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool RemoverCertificado(long id, string cpf, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            var certificado = _hardSkillRepository.ObterCertificadoPorId(id);

            if (certificado == null)
            {
                throw new Exception("Certificado não encontrado.");
            }
            if (certificado.ativo == false)
            {
                throw new Exception("Certificado já foi removido.");
            }

            if (certificado.CodigoInternoColaborador != cpf)
            {
                throw new Exception("Este certificado não pertence ao colaborador.");
            }

            certificado.ativo = Convert.ToBoolean(0);
            _hardSkillRepository.AtualizarCertificado(certificado);
            _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.DELETE, null, null, ItemCVEnum.CERTIFICACAO);
            return true;
        }

        public bool AlterarCertificadoPrincipalColaborador(long id, string cpf)
        {
            var certificadoColabRow = ObterColaboradorCompetenciaCertificado(id);

            if (certificadoColabRow == null)
            {
                throw new Exception("Certificado não encontrado.");
            }
            if (certificadoColabRow.Ativo == 0)
            {
                throw new Exception("Certificado já foi removido.");
            }

            if (certificadoColabRow.CpfTbColaboradroCompetencia != cpf)
            {
                throw new Exception("Este certificado não pertence ao colaborador.");
            }

            var antigoPrincipal = ObterColaboradorCompetenciaCertificadoPrincipal(id);

            antigoPrincipal.Principal = 0;
            certificadoColabRow.Principal = 1;

            AtualizarColaboradorCompetenciaCertificado(antigoPrincipal);
            AtualizarColaboradorCompetenciaCertificado(certificadoColabRow);

            return true;
        }

        public ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificado(long idCertificadoCompetencia)
        {
            return _hardSkillRepository.ObterColaboradorCompetenciaCertificado(idCertificadoCompetencia);
        }

        public ColaboradorCompetenciaCertificadoDTO AtualizarColaboradorCompetenciaCertificado(ColaboradorCompetenciaCertificadoDTO colaboradorCompetenciaCertificadoDTO)
        {
            return _hardSkillRepository.AtualizarColaboradorCompetenciaCertificado(colaboradorCompetenciaCertificadoDTO);
        }

        public ColaboradorCompetenciaCertificadoDTO ObterColaboradorCompetenciaCertificadoPrincipal(long idCertificadoCompetencia)
        {
            return _hardSkillRepository.ObterColaboradorCompetenciaCertificadoPrincipal(idCertificadoCompetencia);
        }

        public CompetenciaColaboradorDTO AlterarHardSkillColaborador(string cpf, long id, long? nivelId, bool minhaJornada, string gestorExternoPerfil, string usuarioLogado, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            try
            {
                var competenciaRow = ObterHardSkillPorId(id);

                var competenciaColaboradorRow = ObterHardSkillColaborador(id, cpf);

                if (nivelId != null)
                {
                    var nivelRow = ListarNivelHardSkill().Where(x => x.Id == nivelId).FirstOrDefault();

                    if (nivelRow == null)
                        throw new Exception("Nível do item não encontrado.");
                    competenciaColaboradorRow.Nivel.Id = nivelRow.Id;
                }

                competenciaColaboradorRow.ColaboradorCpf = cpf;
                _hardSkillRepository.AtualizarCompetenciaColaborador(competenciaColaboradorRow);

                if (minhaJornada)
                {
                    SkillsLog logSkill = new()
                    {
                        CodigoInternoColaborador = cpf,
                        GestorExternoPerfil = gestorExternoPerfil,
                        SkillId = id,
                        ItemPerfil = ItemPerfilEnum.COMPETENCIA,
                        NivelId = nivelId,
                        SkillsMovimentacaoId = EnumSkillsMovimentacao.ATUALIZADO,
                        LogAutomatico = true,
                        CodigoInternoColaboradorLogado = usuarioLogado
                    };

                    _competenciaRepository.GravarLogsSkillsMinhaJornada(logSkill);
                }

                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, competenciaColaboradorRow.Competencia.Id, nivelId, ItemCVEnum.HARDSKILL);
                return competenciaColaboradorRow;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<long> ListarIdsPorHardSkillId(long id)
        {
            try
            {
                return _hardSkillRepository.ListarIdsPorHardSkillId(id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<CertificadoDTO> InserirCertificadoHardSkillColaborador(string cpf, long? competenciaColaboradorId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            bool inseriuCertificado = false;
            long certificadoId = 0;
            try
            {
                if (competenciaColaboradorId != null)
                {
                    var skill = ObterHardSkillColaborador((long)competenciaColaboradorId) ?? throw new Exception("Só é possível enviar certificados para competências que o colaborador possuí.");
                }
                var certificado = new CertificadoDTO();

                if (imagem != null)
                {
                    var nomeArquivo = cpf + "_COMPETENCIA_" + DateTime.Now.ToString("yyyyMMddHHmmssF");

                    if (tipo == TipoCertificadoEnum.IMAGEM)
                    {
                        certificado.Path = nomeArquivo + ".png";
                    }
                    else
                    {
                        certificado.Path = nomeArquivo + ".pdf";
                    }
                }
                else
                {
                    certificado.Path = "";
                }

                certificado.descricao = descricao;
                certificado.instituicao = instituicao;
                certificado.conclusao = conclusao;
                certificado.cargaHoraria = cargaHoraria;

                certificadoId = _hardSkillRepository.InserirCertificadoHardSkillColaborador(certificado, cpf, competenciaColaboradorId);
                inseriuCertificado = true;

                if (!String.IsNullOrEmpty(certificado.Path) && imagem != null)
                {
                    var pathPastaCertificadoS3 = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + certificado.Path;

                    if (certificado != null)
                    {
                        if (tipo == TipoCertificadoEnum.IMAGEM)
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, imagem);
                        }
                        else
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, imagem);

                            var thumb = GeradorThumbUtil.ConverterPDF(imagem, 150);

                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3.Replace(".pdf", "_thumb.pdf"), thumb);
                        }
                        certificado.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + pathPastaCertificadoS3;
                    }
                }
                _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.INSERT, null, null, ItemCVEnum.CERTIFICACAO);
                return certificado;
            }
            catch (Exception)
            {
                if (inseriuCertificado)
                {
                    _hardSkillRepository.RemoverCertificadoHardSkillColaboradorRollback(certificadoId, competenciaColaboradorId);
                }
                throw;
            }
        }

        public async Task<CertificadoDTO> AlterarCertificado(string cpf, long certificadoId, byte[] imagem, TipoCertificadoEnum tipo, DateTime conclusao, string descricao, string instituicao, int cargaHoraria, OrigemAlteracaoCVEnum origem = OrigemAlteracaoCVEnum.MANUAL)
        {
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var certificado = _hardSkillRepository.ObterCertificadoPorId(certificadoId);

                    if (certificado.IdCertificado == null)
                    {
                        throw new Exception("Certificado nao encontrado!");
                    }

                    if (certificado != null)
                    {
                        var nomeArquivo = cpf + "_COMPETENCIA_" + DateTime.Now.ToString("yyyyMMddHHmmssF");

                        if (tipo == TipoCertificadoEnum.IMAGEM)
                        {
                            certificado.Path = nomeArquivo + ".png";
                        }
                        else if (tipo == TipoCertificadoEnum.PDF)
                        {
                            certificado.Path = nomeArquivo + ".pdf";
                        }

                        var pathPastaCertificadoS3 = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + certificado.Path;

                        if (tipo == TipoCertificadoEnum.IMAGEM)
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, imagem);
                        }
                        else if (tipo == TipoCertificadoEnum.PDF)
                        {
                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3, imagem);

                            var thumb = GeradorThumbUtil.ConverterPDF(imagem, 150);

                            await _uploadFilesClient.UploadFile(pathPastaCertificadoS3.Replace(".pdf", "_thumb.pdf"), thumb);
                        }

                        certificado.descricao = descricao;
                        certificado.instituicao = instituicao;
                        certificado.conclusao = conclusao;
                        certificado.cargaHoraria = cargaHoraria;
                        _hardSkillRepository.AtualizarCertificado(certificado);
                        certificado.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + pathPastaCertificadoS3;
                    }
                    else
                    {
                        certificado.descricao = descricao;
                        certificado.instituicao = instituicao;
                        certificado.conclusao = conclusao;
                        certificado.cargaHoraria = cargaHoraria;
                        _hardSkillRepository.AtualizarCertificado(certificado);
                    }
                    dbTrans.Commit();
                    _historicoCvRepository.InserirHistoricoCV(cpf, origem, TipoItemCVEnum.UPDATE, null, null, ItemCVEnum.CERTIFICACAO);
                    return certificado;
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
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

        public CompetenciasSumarioResult SumarioCompetencias(FiltroSumarioCompetenciasParam param)
        {
            try
            {
                ValidaAcessoSumarioCompetencia();

                if (param.UnidadeId.Count < 1) throw new Exception("Necessário selecionar uma unidade");
                if (param.competencia.Count < 1 && param.Cpf.Count < 1) throw new Exception("Necessário selecionar um colaborador ou skill");
                var token = _token.Base64(_aspNetUser.GetUsuarioLogado().Token);
                var orgId = _aspNetUser.GetUsuarioLogado().OrgId;
                return _hardSkillRepository.BuscarColaboradorSumario(param, token, orgId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ItensSumarioResult SumarioCompetenciasListarSkills()
        {
            try
            {
                ValidaAcessoSumarioCompetencia();

                return _hardSkillRepository.ListarSkillsSumario();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<long> ListarHardSkillAtribuidas(string cpfColaborador)
        {
            return _hardSkillRepository.ListarCodigoCompetenciasColaborador(cpfColaborador);
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

        public List<UnidadesDTO> ListUnidadesPorOrg(string token, int orgId)
        {
            var unidades = _foursysClient.ListarUnidadesPorOrg(token, orgId).Result.ListaUnidades;
            return unidades.OrderBy(x => x.Id).ToList();
        }

        public CompetenciaColaboradorDTO ObterHardSkillColaborador(long competenciaId, string cpf)
        {
            var ret = new CompetenciaColaboradorDTO();
            var competenciaColaborador = _hardSkillRepository.ObterCompetenciaColaborador(competenciaId, cpf);
            if (competenciaColaborador == null)
            {
                throw new ArgumentException("O colaborador não possui essa hard skill.");
            }
            var competencia = ObterHardSkillPorId(competenciaColaborador.IdCompetencia);
            ret.Competencia = competencia;
            ret.Id = competenciaColaborador.Id;
            ret.Nivel.Id = competenciaColaborador.IdNivel;
            ret.Endosso = null;
            ret.Data = competenciaColaborador.Data;
            ret.ColaboradorCpf = competenciaColaborador.ColaboradorCpf;
            ret.Certificados = ListarCertificadosHardSkillColaborador(competenciaColaborador.Id);
            return ret;
        }

        public CompetenciaColaboradorDTO ObterHardSkillColaborador(long competenciaColaboradorId)
        {
            var ret = new CompetenciaColaboradorDTO();
            var competenciaColaborador = _hardSkillRepository.ObterCompetenciaColaborador(competenciaColaboradorId);
            if (competenciaColaborador == null)
            {
                throw new ArgumentException("O colaborador não possui essa hard skill.");
            }
            var competencia = ObterHardSkillPorId(competenciaColaborador.IdCompetencia);
            ret.Competencia = competencia;
            ret.Id = competenciaColaborador.Id;
            ret.Nivel.Id = competenciaColaborador.IdNivel;
            ret.Endosso = null;
            ret.Data = competenciaColaborador.Data;
            ret.ColaboradorCpf = competenciaColaborador.ColaboradorCpf;
            ret.Certificados = ListarCertificadosHardSkillColaborador(competenciaColaborador.Id);
            return ret;
        }

        public List<CompetenciaColaboradorDTO> ListarHardSkillColaboradorCompleto(string cpf)
        {
            var codigosCompetenciasColab = _hardSkillRepository.ListarCodigoCompetenciasColaborador(cpf);
            var ret = new List<CompetenciaColaboradorDTO>();
            var niveis = ListarNivelHardSkill();
            foreach (var id in codigosCompetenciasColab)
            {
                var competenciaColab = ObterHardSkillColaborador(id, cpf);
                competenciaColab.Nivel = niveis.Where(x => x.Id == competenciaColab.Nivel.Id).FirstOrDefault();
                ret.Add(competenciaColab);
            }
            return ret.OrderBy(x => x.Competencia.Descricao).ToList();
        }

        public List<CertificadoDTO> ListarCertificadosHardSkillColaborador(long competenciaColaboradorId)
        {
            try
            {
                var ret = _hardSkillRepository.ListarCertificadosHardSkillColaborador(competenciaColaboradorId);
                foreach (var cert in ret)
                {
                    cert.Path = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + cert.Path;
                }
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CertificadoColaboradorDTO> ListaCertificadoColaborador(string cpfColaborador)
        {
            try
            {
                var competenciasColaborador = ListarHardSkillColaboradorCompleto(cpfColaborador);
                var certificadosColaborador = _competenciaColaboradorRepository.GetCertificadoColaborador(cpfColaborador);
                var retCertificados = new List<CertificadoColaboradorDTO>();
                var certificadosRet = certificadosColaborador.Select(x => new CertificadoColaboradorDTO
                {
                    Competencia = competenciasColaborador
                        .Where(y => y.Certificados.Where(z => z.IdCertificado == x.IdCertificado).Any() == true)
                        ?.FirstOrDefault()
                        ?.Competencia,
                    Certificado = x
                }).ToList();
                certificadosRet.ForEach(x => x.Certificado.Path = String.IsNullOrEmpty(x.Certificado.Path) ? null : VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + x.Certificado.Path);
                certificadosRet = certificadosRet.OrderByDescending(x => x.Certificado.conclusao ?? DateTime.MinValue).ToList();
                return certificadosRet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<CertificadoColaboradorDTO> ListaCertificadoColaboradorPorCodigoInterno(string cpfColaborador, string tokenSistema)
        {
            try
            {
                if (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO) != tokenSistema)
                    throw new Exception("Não autorizado");
                var competenciasColaborador = ListarHardSkillColaboradorCompleto(cpfColaborador);
                var certificadosColaborador = _competenciaColaboradorRepository.GetCertificadoColaborador(cpfColaborador);
                var retCertificados = new List<CertificadoColaboradorDTO>();
                var certificadosRet = certificadosColaborador.Select(x => new CertificadoColaboradorDTO
                {
                    Competencia = competenciasColaborador
                        .Where(y => y.Certificados.Where(z => z.IdCertificado == x.IdCertificado).Any() == true)
                        ?.FirstOrDefault()
                        ?.Competencia,
                    Certificado = x
                }).ToList();
                certificadosRet.ForEach(x => x.Certificado.Path = String.IsNullOrEmpty(x.Certificado.Path) ? null : VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CERTIFICADOS) + x.Certificado.Path);
                certificadosRet = certificadosRet.OrderByDescending(x => x.Certificado.conclusao ?? DateTime.MinValue).ToList();
                return certificadosRet;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ListaCompetenciaResult ListarCompetenciaNaoAtribuidas(string busca, int cursor, int limite, string cpf)
        {
            var listaIds = ListarHardSkillAtribuidas(cpf);

            var retService = ListarHardSkill(busca, cursor, limite);

            if (listaIds.Count > 0)
            {
                retService = retService.Where(x => !listaIds.Contains(x.Id)).ToList();
            }

            var ret = new ListaCompetenciaResult();

            ret.Competencias = retService.Select(x => new ItemPerfilDTO
            {
                Descricao = x.Descricao,
                Id = x.Id,
                Pendente = x.Pendente
            }).ToList();

            ret.Nivel = ListarNivelHardSkill();

            return ret;
        }

        public async Task<List<KeyValuePair<string, long>>> GetHardSkillInfoByDescricaoAsync(List<string> skills)
        {
            return await _skillGenericService.GetSkillInfoByDescricaoAsync(skills, TipoSkillEnum.HARDSKILL, _aspNetUser.GetUsuarioLogado().Cpf);
        }
    }
}