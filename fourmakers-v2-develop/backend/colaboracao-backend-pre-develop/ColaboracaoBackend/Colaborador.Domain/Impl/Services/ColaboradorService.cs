using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Util;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using Core.Domain;
using Core.Domain.Colaborador;
using Core.Domain.Curriculo;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel;
using Core.DomainModel.Colaborador;
using Core.DomainModel.Org;
using Core.DomainModel.SSO;
using DataTransferObject.Domain.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Carga;
using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Comentario;
using DataTransferObject.Domain.Dependentes;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Endosso;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.Org;
using DataTransferObject.Domain.TemplateEmail;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colaboracao.Helper.Enum;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Foursys;
using TemplateOrg.Constantes;
using Colaboracao.Core.Interfaces;
using SRS.Infra.Constantes;

namespace Colaborador.Domain.Impl.Services
{
    [LogDomainClass]
    public class ColaboradorService : IColaboradorService
    {
        private readonly IColaboradorDtoRepository _colaboradorDtoRepository;
        private readonly IDependenteDtoRepository _dependenteDtoRepository;
        private readonly IEnderecoDtoRepository _enderecoDtoRepository;
        private readonly IConfiguration _configuration;
        private readonly IFotoDtoRepository _fotoDtoRepository;
        private readonly ISimpleColaboradorDtoRepository _simpleColaboradorDtoRepository;
        private readonly IStatusDtoRepository _statusDtoRepository;
        private readonly IAspNetUser _aspNetUser;
        private readonly ICompetenciaClient _competenciaClient;
        private readonly IFormacaoClient _formacaoClient;
        private readonly IDominioClient _dominioClient;
        private readonly IMetodologiaClient _metodologiaClient;
        private readonly IInteresseClient _interesseClient;
        private readonly IHobbyClient _hobbyClient;
        private readonly ISoftskillClient _softskillClient;
        private readonly IEndossoClient _endossoClient;
        private readonly IFirebaseClient _firebaseClient;
        private readonly IUploadFilesClient _uploadFilesClient;
        private readonly IComentarioClient _comentarioClient;
        private readonly ILogCore _log;
        private readonly ICCHClient _cchClient;
        private readonly IUsuarioClient _usuarioClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IForcaPerfilRepository _forcaPerfilRepository;
        private readonly ICurriculoColaboradorRespository _curriculoColaboradorRespository;
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly IEstatisticasRepository _estatisticasRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly ISSORepository _ssoRepository;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly IColaboradorValidadorService _colaboradorValidadorService;
        private readonly IContatoEmergenciaRepository _contatoEmergenciaRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IColaboradorSugestaoRepository _colaboradorSugestaoRepository;
        private readonly ITokens _token;
        private readonly IIdiomaClient _idiomaClient;
        private readonly IExperienciaProfissionalService _experienciaProfissionalService;
        private readonly IEscolaridadeColaboradorService _escolaridadeColaboradorService;
        private readonly IBancoDeTalentosService _bancoDeTalentosService;
        private readonly IDBConnectionUnitOfWork _dBConnectionUnitOfWork;
        private readonly ICidadaniaRepository _cidadaniaRepository;
        private readonly IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService;
        private readonly IColaboradorGrupoAcessoConfiguracaoService _colaboradorGrupoAcessoConfiguracao;
        private readonly IColaboradorDapperRepository _colaboradorRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly IColaboradorOrgRepository _colaboradorOrgRepository;
        private readonly IClienteOrgRepository _clienteOrgRepository;
        private readonly IRestricaoDeAcessoService _restricaoDeAcessoService;

        public ColaboradorService(IColaboradorDtoRepository colaboradorDtoRepository,
                                  IDependenteDtoRepository dependenteDtoRepository,
                                  IEnderecoDtoRepository enderecoDtoRepository,
                                  IConfiguration configuration,
                                  IFotoDtoRepository fotoDtoRepository,
                                  ISimpleColaboradorDtoRepository simpleColaboradorDtoRepository,
                                  IStatusDtoRepository statusDtoRepository,
                                  IAspNetUser aspNetUser,
                                  ICompetenciaClient competenciaClient,
                                  IFormacaoClient formacaoClient,
                                  IDominioClient dominioClient,
                                  IMetodologiaClient metodologiaClient,
                                  IInteresseClient interesseClient,
                                  IHobbyClient hobbyClient,
                                  ISoftskillClient softskillClient,
                                  IEndossoClient endossoClient,
                                  IFirebaseClient firebaseClient,
                                  IUploadFilesClient uploadFilesClient,
                                  IComentarioClient comentarioClient,
                                  ILogCore log,
                                  ICCHClient cchClient,
                                  IUsuarioClient usuarioClient,
                                  IUnitOfWork unitOfWork,
                                  IForcaPerfilRepository forcaPerfilRepository,
                                  ICurriculoColaboradorRespository curriculoColaboradorRespository,
                                  IBuscaColaboradorRepository buscaColaboradorRepository,
                                  IEstatisticasRepository estatisticasRepository,
                                  IUsuarioColaboradorRepository usuarioColaboradorRepository,
                                  ISSORepository ssoRepository,
                                  IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                  IOrgRepository orgRepository, IColaboradorValidadorService colaboradorValidadorService,
                                  IContatoEmergenciaRepository contatoEmergenciaRepository,
                                  ITemplateRepository templateRepository,
                                  IColaboradorSugestaoRepository colaboradorSugestaoRepository,
                                  ITokens token,
                                  IIdiomaClient idiomaClient,
                                  IExperienciaProfissionalService experienciaProfissionalService,
                                  IEscolaridadeColaboradorService escolaridadeColaboradorService,
                                  IBancoDeTalentosService bancoDeTalentosService,
                                  IDBConnectionUnitOfWork dBConnectionUnitOfWork,
                                  ICidadaniaRepository cidadaniaRepository,
                                  IBuscaParametroConfiguracaoService buscaParametroConfiguracaoService,
                                  IColaboradorGrupoAcessoConfiguracaoService colaboradorGrupoAcessoConfiguracao,
                                  IColaboradorDapperRepository colaboradorRepository,
                                  ICandidaturaRepository candidaturaRepository, IColaboradorOrgRepository colaboradorOrgRepository, IClienteOrgRepository clienteOrgRepository, IRestricaoDeAcessoService restricaoDeAcessoService)
        {
            _colaboradorDtoRepository = colaboradorDtoRepository;
            _dependenteDtoRepository = dependenteDtoRepository;
            _enderecoDtoRepository = enderecoDtoRepository;
            _configuration = configuration;
            _fotoDtoRepository = fotoDtoRepository;
            _simpleColaboradorDtoRepository = simpleColaboradorDtoRepository;
            _statusDtoRepository = statusDtoRepository;
            _aspNetUser = aspNetUser;
            _competenciaClient = competenciaClient;
            _formacaoClient = formacaoClient;
            _dominioClient = dominioClient;
            _metodologiaClient = metodologiaClient;
            _interesseClient = interesseClient;
            _hobbyClient = hobbyClient;
            _endossoClient = endossoClient;
            _firebaseClient = firebaseClient;
            _uploadFilesClient = uploadFilesClient;
            _comentarioClient = comentarioClient;
            _log = log;
            _cchClient = cchClient;
            _usuarioClient = usuarioClient;
            _unitOfWork = unitOfWork;
            _softskillClient = softskillClient;
            _forcaPerfilRepository = forcaPerfilRepository;
            _curriculoColaboradorRespository = curriculoColaboradorRespository;
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _estatisticasRepository = estatisticasRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _ssoRepository = ssoRepository;
            _usuarioLogado = _aspNetUser.GetUsuarioLogado();
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _orgRepository = orgRepository;
            _colaboradorValidadorService = colaboradorValidadorService;
            _contatoEmergenciaRepository = contatoEmergenciaRepository;
            _templateRepository = templateRepository;
            _colaboradorSugestaoRepository = colaboradorSugestaoRepository;
            _token = token;
            _idiomaClient = idiomaClient;
            _experienciaProfissionalService = experienciaProfissionalService;
            _escolaridadeColaboradorService = escolaridadeColaboradorService;
            _bancoDeTalentosService = bancoDeTalentosService;
            _dBConnectionUnitOfWork = dBConnectionUnitOfWork;
            _cidadaniaRepository = cidadaniaRepository;
            _buscaParametroConfiguracaoService = buscaParametroConfiguracaoService;
            _colaboradorGrupoAcessoConfiguracao = colaboradorGrupoAcessoConfiguracao;
            _colaboradorRepository = colaboradorRepository;
            _candidaturaRepository = candidaturaRepository;
            _colaboradorOrgRepository = colaboradorOrgRepository;
            _clienteOrgRepository = clienteOrgRepository;
            _restricaoDeAcessoService = restricaoDeAcessoService;
        }

        public DependentesDTO AlterarDadosDependenteColaborador(AlterarDependentesDTO colabInfo, string cpf)
        {
            if (String.IsNullOrEmpty(cpf))
                throw new Exception("O número de cpf é obrigatório.");

            var dto = new DependentesDTO
            {
                Id = colabInfo.Id,
                NomeCompleto = colabInfo.NomeCompleto,
                DataNascimento = colabInfo.DataNascimento.GetValueOrDefault(),
                Rg = colabInfo.Rg,
                Cpf = colabInfo.Cpf,
                PortadorDeficiencia = colabInfo.PortadorDeficiencia,
                RequerAjudaQual = colabInfo.RequerAjudaQual,
                tipoDependenteId = colabInfo.TipoDependente?.Id ?? 0,
                Ativo = 1
            };

            return _dependenteDtoRepository.UpdateModel(dto, cpf);
        }

        public DependentesDTO AdicionarDadosDependenteColaborador(AdicionarDependentesDTO colabInfo, string cpf)
        {
            if (cpf != _usuarioLogado.Cpf)
                throw new Exception("Apenas o usuário logado pode alterar o perfil");
            if (String.IsNullOrEmpty(cpf))
                throw new Exception("O número de cpf é obrigatório.");

            var dto = new DependentesDTO
            {
                NomeCompleto = colabInfo.NomeCompleto,
                DataNascimento = colabInfo.DataNascimento,
                Rg = colabInfo.Rg,
                Cpf = colabInfo.Cpf,
                PortadorDeficiencia = colabInfo.PortadorDeficiencia,
                RequerAjudaQual = colabInfo.RequerAjudaQual,
                tipoDependenteId = colabInfo.TipoDependenteId
            };

            return _dependenteDtoRepository.SaveModel(dto, cpf);
        }

        public void RemoveDependenteColaborador(long dependenteId, string cpf)
        {
            var dto = new DependentesDTO { Id = dependenteId };
            _dependenteDtoRepository.DeleteModel(dto, cpf);
        }

        public async Task<bool> AlterarFotoColaborador(string cpf, int orgId, byte[] imagem, bool gerarThumb)
        {
            if (imagem == null)
            {
                throw new Exception("Imagem inválida.");
            }
            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var pathImagem = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_FOTO_COLABORADOR);
                    var data = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                    var nomeArquivo = pathImagem + "foto_" + cpf + "_" + data;
                    if (gerarThumb)
                    {
                        var thumb = GeradorThumbUtil.Converter(imagem, 200);
                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb.png", thumb);
                        var thumb50 = GeradorThumbUtil.Converter(imagem, 50);
                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb50.png", thumb50);
                        var thumb25 = GeradorThumbUtil.Converter(imagem, 25);
                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb25.png", thumb25);
                    }
                    await _uploadFilesClient.UploadFile(nomeArquivo + ".png", imagem);

                    var fotoId = _fotoDtoRepository.SaveFoto(nomeArquivo + ".png");
                    var colaboradorDto = _colaboradorDtoRepository.GetModelByKey(new ColaboradorDTO { Cpf = cpf });
                    _colaboradorDtoRepository.UpdateFotoModel(colaboradorDto);

                    dbTrans.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    dbTrans.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> AlterarDadosColaboradorBase(ColaboradorDTO colabInfo, int orgId, byte[] imagem)
        {
            colabInfo.Cpf = _usuarioLogado.Cpf;

            var colaboradorBanco = _colaboradorDtoRepository.GetModelByKey(new ColaboradorDTO { Cpf = colabInfo.Cpf });

            if (String.IsNullOrEmpty(colabInfo.Cpf))
            {
                throw new Exception("O número de cpf é obrigatório.");
            }

            if (colaboradorBanco == null)
            {
                throw new Exception("Colaborador não encontrado no sistema.");
            }

            if (colabInfo != null)
            {
                var enderecoBanco = _enderecoDtoRepository.GetModelByKey(colaboradorBanco.Cpf);

                if (enderecoBanco == null)
                {
                    var enderecoNovo = MontaEnderecoRow(colabInfo.Endereco);
                    _enderecoDtoRepository.SaveModel(enderecoNovo);
                }
                else
                {
                    var enderecoAtualizado = MontaEnderecoRow(colabInfo.Endereco, enderecoBanco);
                    _enderecoDtoRepository.UpdateModel(enderecoAtualizado);
                }
            }

            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var pathImagem = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_FOTO_COLABORADOR);
                    var data = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
                    var nomeArquivo = pathImagem + "foto_" + colabInfo.Cpf + "_" + data;

                    if (imagem != null)
                    {
                        await _uploadFilesClient.UploadFile(nomeArquivo + ".png", imagem);

                        var thumb = GeradorThumbUtil.Converter(imagem, 200);

                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb.png", thumb);

                        var thumb50 = GeradorThumbUtil.Converter(imagem, 50);

                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb50.png", thumb50);

                        var thumb25 = GeradorThumbUtil.Converter(imagem, 25);

                        await _uploadFilesClient.UploadFile(nomeArquivo + "_thumb25.png", thumb25);

                        var fotoId = _fotoDtoRepository.SaveFoto(nomeArquivo + ".png");
                    }

                    dbTrans.Commit();
                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    throw;
                }

                try
                {
                    colaboradorBanco.Passaporte = colabInfo.Passaporte;
                    colaboradorBanco.ContatoPrincipalDDI = colabInfo.ContatoPrincipalDDI;
                    colaboradorBanco.ContatoPrincipal = colabInfo.ContatoPrincipal;
                    colaboradorBanco.Passaporte = colabInfo.Passaporte;
                    colaboradorBanco.EstadoCivil = colabInfo.EstadoCivil;
                    colaboradorBanco.Genero = colabInfo.Genero;
                    colaboradorBanco.Etnia = colabInfo.Etnia;
                    colaboradorBanco.OrientacaoSexual = colabInfo.OrientacaoSexual;
                    colaboradorBanco.EmailAlternativo = colabInfo.EmailAlternativo;
                    colaboradorBanco.PessoaRefugiada = colabInfo.PessoaRefugiada;
                    colaboradorBanco.Candidato = colabInfo.Candidato;
                    colaboradorBanco.DocumentoColaborador = colabInfo.DocumentoColaborador;
                    _colaboradorDtoRepository.UpdateModel(colaboradorBanco);
                }
                catch (Exception)
                {
                    throw;
                }

                return true;
            }
        }

        public List<ColaboradorDTO> BuscarColaborador(string cpf, int orgId, string busca, int cursor, int limite, out int totalResultsCount, int candidato = 0)
        {
            try
            {
                var colaboradorBanco = _colaboradorDtoRepository.GetModelByKey(new ColaboradorDTO { Cpf = cpf });

                if (colaboradorBanco == null)
                    throw new Exception("Colaborador não encontrado no sistema.");

                var ret = new List<ColaboradorDTO>();
                if (String.IsNullOrEmpty(busca))
                {
                    var colaboradoresBanco = _colaboradorDtoRepository.BuscarColaboradores(cpf, cursor, limite, candidato, orgId, out totalResultsCount);

                    foreach (var row in colaboradoresBanco)
                    {
                        ret.Add(_buscaColaboradorRepository.GetColaborador(row.Cpf, orgId));
                    }
                }
                else
                {
                    var deslocamentoBusca = cursor + limite;
                    var retBusca = new List<BuscaColaboradorDTO>();

                    retBusca = retBusca.Union(MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO1, cpf, candidato), new ColaboradorBuscaComparer()).ToList();
                    var colaboradores = BuscaRowsColaborador(cpf, deslocamentoBusca, retBusca, candidato).ToList();
                    if (colaboradores.Count() < deslocamentoBusca)
                    {
                        retBusca = retBusca.Union(MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO2, cpf, candidato), new ColaboradorBuscaComparer()).ToList();
                        colaboradores.AddRange(BuscaRowsColaborador(cpf, deslocamentoBusca, retBusca, candidato));
                        colaboradores = removeRepetidos(colaboradores);
                    }
                    if (colaboradores.Count() < deslocamentoBusca)
                    {
                        retBusca = retBusca.Union(MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO3, cpf, candidato), new ColaboradorBuscaComparer()).ToList();
                        colaboradores.AddRange(BuscaRowsColaborador(cpf, deslocamentoBusca, retBusca, candidato));
                        colaboradores = removeRepetidos(colaboradores);
                    }
                    if (colaboradores.Count() < deslocamentoBusca)
                    {
                        retBusca = retBusca.Union(MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO4, cpf, candidato), new ColaboradorBuscaComparer()).ToList();
                        colaboradores.AddRange(BuscaRowsColaborador(cpf, deslocamentoBusca, retBusca, candidato));
                        colaboradores = removeRepetidos(colaboradores);
                    }

                    totalResultsCount =
                        //0;
                        MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO1, cpf, candidato).Count
                            + MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO2, cpf, candidato).Count
                            + MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO3, cpf, candidato).Count
                            + MontaGrupoPesquisa(busca, GrupoPesquisa.GRUPO4, cpf, candidato).Count;

                    colaboradores = colaboradores.Skip(cursor).Take(limite).ToList();
                    foreach (var row in colaboradores)
                    {
                        ret.Add(_buscaColaboradorRepository.GetColaborador(row.Cpf, orgId));
                    }
                }
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<ColaboradorDTO> removeRepetidos(List<ColaboradorDTO> rows)
        {
            var ret = new List<ColaboradorDTO>();
            foreach (var row in rows)
            {
                if (ret.Where(x => x.Cpf == row.Cpf).Count() == 0)
                    ret.Add(row);
            }
            return ret;
        }

        public List<ColaboradorDTO> BuscarColaborador(string cpf, int orgId, string busca, int cursor, int limite, out int filteredResultsCount, out int totalResultsCount)
        {
            try
            {
                var ret = BuscarColaborador(cpf, orgId, busca, cursor, limite, out totalResultsCount);
                filteredResultsCount = ret.Count();
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ColaboradorDTO> BuscarCandidato(string cpf, int orgId, string busca, int cursor, int limite, out int filteredResultsCount, out int totalResultsCount)
        {
            try
            {
                var ret = BuscarColaborador(cpf, orgId, busca, cursor, limite, out totalResultsCount, 1);
                filteredResultsCount = ret.Count();
                return ret;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IEnumerable<ColaboradorDTO> BuscaRowsColaborador(string cpfSolicitante, int deslocamentoBusca, List<BuscaColaboradorDTO> lstBusca, int candidato = 0)
        {
            var lstCpf = lstBusca.Select(x => x.CpfColaborador).ToList();
            return _colaboradorDtoRepository.BuscaRowsColaborador(lstCpf, cpfSolicitante, deslocamentoBusca, candidato);
        }

        private List<BuscaColaboradorDTO> MontaGrupoPesquisa(string busca, GrupoPesquisa grupo, string cpf, int candidato)
        {
            var ret = new List<BuscaColaboradorDTO>();
            var coluna = string.Empty;

            switch (grupo)
            {
                case GrupoPesquisa.GRUPO1:
                    coluna = "BuscaGR1";
                    break;

                case GrupoPesquisa.GRUPO2:
                    coluna = "BuscaGR2";
                    break;

                case GrupoPesquisa.GRUPO3:
                    coluna = "BuscaGR3";
                    break;

                case GrupoPesquisa.GRUPO4:
                    coluna = "BuscaGR4";
                    break;
            }

            var retSearchText = _buscaColaboradorRepository.BuscaColaborador(coluna, busca, candidato, cpf);

            foreach (var row in retSearchText)
            {
                ret.Add(new BuscaColaboradorDTO
                {
                    Grupo = grupo,
                    CpfColaborador = row
                });
            }
            return ret;
        }

        public ColaboradorDTO BuscarDadosColaborador(string cpf, int orgId, string cpfRequest, string token)
        {
            if (cpf != cpfRequest)
            {
                ValidaAcessoDadosColaborador(cpfRequest, orgId, FuncionalidadeSistemaEnum.DADOS_COLABORADOR);
            }

            var result = _buscaColaboradorRepository.GetColaborador(cpf, orgId);
            result.Cidadanias = _cidadaniaRepository.ListarCidadaniasColaborador(cpf).GetAwaiter().GetResult();
            var ultimaPretensaoModelo = _candidaturaRepository.ObterUltimaPretensaoModeloInformadaColaboradorAsync(cpf).GetAwaiter().GetResult();
            if (ultimaPretensaoModelo != null)
            {
                result.UltimaPretensaoSalarial = ultimaPretensaoModelo.UltimaPretensaoSalarial;
                result.UltimoModeloTrabalhoId = ultimaPretensaoModelo.UltimoModeloTrabalhoId;
                result.UltimoModeloTrabalhoDescricao = ultimaPretensaoModelo.UltimoModeloTrabalhoDescricao;
            }
            return result;
        }

        public List<DependentesDTO> ListarDadosColaboradorDependente(string cpf)
        {
            return _dependenteDtoRepository.GetModel(cpf);
        }

        public RedeColaboradorDTO BuscarRedeColaborador(string cpf, int orgId, string cpfRequest, string token)
        {
            var ret = new RedeColaboradorDTO
            {
                Seguidores = new List<ColaboradorDTO>(),
                Seguindo = new List<ColaboradorDTO>(),
            };

            var seguidores = _colaboradorDtoRepository.GetSeguidores(cpf);
            if (seguidores != null && seguidores.Count > 0)
            {
                foreach (var seguidor in seguidores)
                {
                    ret.Seguidores.Add(BuscarDadosColaborador(seguidor, orgId, cpfRequest, token));
                }
            }

            var seguindoRows = _colaboradorDtoRepository.GetSeguindo(cpf);
            if (seguindoRows != null && seguindoRows.Count > 0)
            {
                foreach (var seguindoRow in seguindoRows)
                {
                    ret.Seguindo.Add(BuscarDadosColaborador(seguindoRow, orgId, cpfRequest, token));
                }
            }

            return ret;
        }

        private EnderecoDTO MontaEnderecoRow(EnderecoDTO endereco, EnderecoDTO enderecoExistente = null)
        {
            var dto = enderecoExistente ?? new EnderecoDTO();
            dto.Bairro = endereco?.Bairro;
            dto.Cep = endereco?.Cep;
            dto.Cidade = endereco?.Cidade;
            dto.Complemento = endereco?.Complemento;
            dto.Endereco = endereco?.Endereco;
            dto.Estado = endereco?.Estado;
            dto.Numero = endereco?.Numero;
            dto.ComQuemMora = endereco?.ComQuemMora;
            return dto;
        }

        public async Task<PerfilProfissionalDTO> BuscarPerfilProfissional(string cpf)
        {
            if (cpf != _usuarioLogado.Cpf && _buscaColaboradorRepository.ValidaAcesso(_usuarioLogado.Cpf, _usuarioLogado.OrgId, _usuarioLogado.Token) == false)
            {
                throw new UnauthorizedAccessException("Acesso negado");
            }

            var taskCompetencias = _competenciaClient.ListarCompetenciasColaborador(cpf, _usuarioLogado.Token);
            //var taskFormacoes = _formacaoClient.ListarFormacoesColaborador(cpf, _aspNetUser.GetUsuarioLogado().Token);
            var taskDominios = _dominioClient.ListarDominiosColaborador(cpf, _usuarioLogado.Token);
            var taskMetodologias = _metodologiaClient.ListarMetodologiasColaborador(cpf, _usuarioLogado.Token);
            //var taskInteresses = _interesseClient.ListarInteressesColaborador(cpf, _aspNetUser.GetUsuarioLogado().Token);
            //var taskHobbies = _hobbyClient.ListarHobbiesColaborador(cpf, _aspNetUser.GetUsuarioLogado().Token);
            var taskSoftskills = _softskillClient.ListarSoftskillsColaborador(cpf, _aspNetUser.GetUsuarioLogado().Token);
            var taskIdiomas = _idiomaClient.ListarIdiomaColaborador(cpf, _usuarioLogado.Token);
            var taskCertificados = _competenciaClient.ListarCertificadoColaboradorPorCodigoInterno(cpf, VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO));
            Task.WaitAll(taskCompetencias, taskDominios, taskMetodologias, taskSoftskills, taskCertificados, taskIdiomas);

            return new PerfilProfissionalDTO
            {
                Competencias = taskCompetencias.Result,
                //Formacoes = taskFormacoes.Result,
                Dominios = taskDominios.Result,
                Metodologias = taskMetodologias.Result,
                //Interesses = taskInteresses.Result,
                //Hobbies = taskHobbies.Result,
                Softskills = taskSoftskills.Result,
                Certificados = taskCertificados.Result.Certificados,
                Idiomas = taskIdiomas.Result,
                ExperienciaEmpresas = _experienciaProfissionalService.ListarExperienciaProfissionalAgrupada("", 0, 32000, cpf, _usuarioLogado.OrgId)
            };
        }

        public async Task<List<PedidoEndossoDTO>> GetPedidosEndossoColaborador(string cpfRequest)
        {
            var endossos = await _endossoClient.ListarEndossosColaborador(cpfRequest, _usuarioLogado.Token);

            if (endossos.Count == 0)
            {
                return null;
            }
            else
            {
                return endossos;
            }
        }

        public async Task<ForcaPerfilDTO> BuscarForcaPerfil(string cpfRequest, int orgId)
        {
            var ret = new ForcaPerfilDTO();

            //Dados academicos
            var nivelEscolaridade = _forcaPerfilRepository.PossuiEscolaridade(cpfRequest);
            var quantidadeCertificados = _forcaPerfilRepository.GetNumeroCertificadosCompetencia(cpfRequest);
            ret.ForcaDadosAcademicos = 0f;
            ret.ForcaDadosAcademicos += nivelEscolaridade ? 20 : 0;
            ret.ForcaDadosAcademicos += quantidadeCertificados * 10;
            if (ret.ForcaDadosAcademicos > 100)
            {
                ret.ForcaDadosAcademicos = 100;
            }

            //Dados profissionais
            var experienciaProfissional = _forcaPerfilRepository.GetQuantidadeExperienciaProfissional(cpfRequest);
            var quandidadeHardSkill = _forcaPerfilRepository.GetQuantidadeCompetencia(cpfRequest);
            var quandidadeSoftskill = _forcaPerfilRepository.GetQuantidadeSoftskill(cpfRequest);
            var possuiMetodologia = _forcaPerfilRepository.PossuiMetodologia(cpfRequest);
            var possuiDominioNegocio = _forcaPerfilRepository.PossuiDominioNegocio(cpfRequest);
            var possuiIdioma = _forcaPerfilRepository.PossuiIdioma(cpfRequest);
            var possuiCv = _forcaPerfilRepository.PossuiCurriculo(cpfRequest);
            ret.ForcaDadosProfissionais = 0f;
            ret.ForcaDadosProfissionais += experienciaProfissional > 0 ? 10 : 0;
            ret.ForcaDadosProfissionais += possuiMetodologia ? 10 : 0;
            ret.ForcaDadosProfissionais += possuiDominioNegocio ? 10 : 0;
            ret.ForcaDadosProfissionais += possuiIdioma ? 10 : 0;
            ret.ForcaDadosProfissionais += possuiCv ? 10 : 0;
            if (quandidadeHardSkill > 6)
            {
                quandidadeHardSkill = 6;
            }
            ret.ForcaDadosProfissionais += quandidadeHardSkill <= 0 ? 0 : 5 * quandidadeHardSkill;
            if (quandidadeSoftskill > 4)
            {
                quandidadeSoftskill = 4;
            }
            ret.ForcaDadosProfissionais += quandidadeSoftskill <= 0 ? 0 : 5 * quandidadeSoftskill;

            //Dados pessoais
            var colabRow = _colaboradorDtoRepository.GetModelByKey(new ColaboradorDTO { Cpf = cpfRequest });
            ret.ForcaDadosPessoais = 0;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.NomeCompleto) ? 0 : 10;
            DateTime dateAux;
            // 5 + 5 +
            ret.ForcaDadosPessoais += colabRow.DataNascimento != null ? 10 : 0;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.Cpf) ? 0 : 10;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.Rg) ? 0 : 10;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.Genero) ? 0 : 5;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.OrientacaoSexual) ? 0 : 5;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.Escolaridade) ? 0 : 10;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.Etnia) ? 0 : 5;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.EmailAlternativo) ? 0 : 5;
            ret.ForcaDadosPessoais += String.IsNullOrEmpty(colabRow.ContatoPrincipal) ? 0 : 10;
            ret.ForcaDadosPessoais += colabRow.Endereco == null ? 0 : 15;

            ret.ForcaDadosAcademicos /= 100f;
            ret.ForcaDadosProfissionais /= 100f;
            ret.ForcaDadosPessoais /= 100f;

            return ret;
        }

        public async Task<List<ItemPerfilResult>> AdicionarCompetenciaColaborador(List<AdicionarRemoverItemDTO> dtos)
        {
            return await _competenciaClient.AdicionarCompetenciaColaborador(dtos, _usuarioLogado.Token);
        }
        public Task RemoveCertificadoCompetenciaColaborador(string cpf, long id)
        {
            return _competenciaClient.RemoveCertificadoCompetenciaColaborador(cpf, id, _usuarioLogado.Token);
        }

        public async Task AlteraCertificadoPrincipalColaborador(string cpf, long id)
        {
            await _competenciaClient.AlteraCertificadoPrincipalColaborador(cpf, id, _aspNetUser.GetUsuarioLogado().Token);
        }

        public async Task<StatusResult> RemoverCompetenciaColaborador(string token, string cpf, long id)
        {
            return await _competenciaClient.RemoverCompetenciaColaborador(token, cpf, id);
        }

        public async Task<List<ItemPerfilResult>> InserirFormacaoColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _formacaoClient.InserirFormacaoColaborador(token, dtos);
        }

        public async Task<StatusResult> RemoverFormacaoColaborador(string token, string cpf, long id)
        {
            return await _formacaoClient.RemoverFormacaoColaborador(token, cpf, id);
        }

        public async Task<StatusResult> RemoverDominioColaborador(string token, string cpf, long id)
        {
            return await _dominioClient.RemoverDominioColaborador(token, cpf, id);
        }

        public async Task<StatusResult> RemoverMetodologiaColaborador(string token, string cpf, long id)
        {
            return await _metodologiaClient.RemoverMetodologiaColaborador(token, cpf, id);
        }

        public async Task<StatusResult> RemoverInteresseColaborador(string token, string cpf, long id)
        {
            return await _interesseClient.RemoverInteresseColaborador(token, cpf, id);
        }

        public async Task<StatusResult> RemoverHobbieColaborador(string token, string cpf, long id)
        {
            return await _hobbyClient.RemoverHobbieColaborador(token, cpf, id);
        }

        public Task<List<ComentarioTipoDTO>> ListarTipoComentarios()
        {
            return _comentarioClient.ListarTipoComentarios(_usuarioLogado.Token);
        }

        public Task<ComentarioDTO> InserirComentario(string cpf, int type, string texto)
        {
            return _comentarioClient.InserirComentario(cpf, type, texto, _usuarioLogado.Token);
        }

        public Task<CertificadoDTO> InserirCertificadoFormacaColaborador(string cpf, long formacaoColaboradorId, byte[] file, TipoCertificadoEnum tipo)
        {
            return _formacaoClient.InserirCertificadoFormacaColaborador(cpf, formacaoColaboradorId, file, tipo, _usuarioLogado.Token);
        }

        public async Task<List<ItemPerfilResult>> InserirDominioColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _dominioClient.InserirDominioColaborador(token, dtos);
        }

        public async Task<List<ItemPerfilResult>> InserirMetodologiaColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _metodologiaClient.InserirMetodologiaColaborador(cpf, dtos);
        }

        public async Task<List<ItemPerfilResult>> InserirInteresseColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _interesseClient.InserirInteresseColaborador(cpf, dtos);
        }

        public async Task<List<ItemPerfilResult>> InserirHobbieColaborador(string cpf, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _hobbyClient.InserirHobbieColaborador(cpf, dtos);
        }

        public SimpleColaboradorDTO BuscarNomeColaborador(string cpfColaborador)
        {
            try
            {
                var colaborador = _colaboradorDtoRepository.BuscarNomeColaborador(new ColaboradorDTO { Cpf = cpfColaborador });

                return new SimpleColaboradorDTO
                {
                    Cpf = colaborador.Cpf,
                    NomeCompleto = colaborador.NomeCompleto
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<ItemPerfilResult> AlterarCompetenciaColaborador(AdicionarRemoverItemDTO dtos)
        {
            return _competenciaClient.AlterarCompetenciaColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<ItemPerfilResult> AlterarFormacaoColaborador(AdicionarRemoverItemDTO dtos)
        {
            return _formacaoClient.AlterarFormacaoColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeFormacaoColaborador(MergeItemPerfilDTO dtos)
        {
            return _formacaoClient.MergeFormacaoColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<ItemPerfilResult> AlterarDominioColaborador(AdicionarRemoverItemDTO dtos)
        {
            return _dominioClient.AlterarDominioColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeCompetenciaColaborador(MergeItemPerfilDTO dtos)
        {
            return _competenciaClient.MergeCompetenciaColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeDominioColaborador(MergeItemPerfilDTO dtos)
        {
            return _dominioClient.MergeDominioColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<ItemPerfilResult> AlterarMetodologiaColaborador(AdicionarRemoverItemDTO dtos)
        {
            return _metodologiaClient.AlterarMetodologiaColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeMetodologiaColaborador(MergeItemPerfilDTO dtos)
        {
            return _metodologiaClient.MergeMetodologiaColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeInteresseColaborador(MergeItemPerfilDTO dtos)
        {
            return _interesseClient.MergeInteresseColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilDTO dtos)
        {
            return _hobbyClient.MergeHobbieColaborador(dtos, _usuarioLogado.Token);
        }

        public ListColaboradoresResult BuscarListaColaboradores(string nomeCompleto)
        {
            var retorno = new ListColaboradoresResult();
            retorno.Colaboradores = _simpleColaboradorDtoRepository.BuscarListaColaboradores(nomeCompleto);
            return retorno;
        }

        private void ValidaAcessoDadosColaborador(string cpf, int orgId, FuncionalidadeSistemaEnum funcionalidadesAcesso)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, funcionalidadesAcesso).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado para Dados dos Colaboradores.");
            }
        }

        private void ValidaAcessoSimuladorRemuneracao(string cpf, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.SIMULADOR_REMUNERACAO).Result;

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Acesso negado. Você não possui permissão para acessar o Simulador de Remuneração.");
            }
        }

        public Task<bool> ValidaAcessoGrupoFuncionalidade(string cpfRequest, FuncionalidadeSistemaEnum enumFuncionalidadeSistema)
        {
            return _usuarioClient.ValidaAcessoGrupoFuncionalidade(cpfRequest, _usuarioLogado.Token, enumFuncionalidadeSistema);
        }

        private class ColaboradorBuscaComparer : IEqualityComparer<BuscaColaboradorDTO>
        {
            public bool Equals(BuscaColaboradorDTO c1, BuscaColaboradorDTO c2)
            {
                return c1.CpfColaborador == c2.CpfColaborador;
            }

            public int GetHashCode(BuscaColaboradorDTO p)
            {
                return p.GetHashCode();
            }
        }

        public List<StatusColaboradorDTO> BuscarStatusColaborador()
        {
            var listaStatusColaborador = new List<StatusColaboradorDTO>();
            var listaColaboradores = _colaboradorDtoRepository.BuscarStatusColaborador();

            foreach (var item in listaColaboradores)
            {
                listaStatusColaborador.Add(new StatusColaboradorDTO
                {
                    Id = item.Status.Id,
                    Descricao = item.Status.Descricao
                });
            }

            return listaStatusColaborador;
        }

        public List<ColaboradorAtivoDTO> BuscarColaboradoresAtivos(int cursor, int limite, out int filteredResultCount, out int totalResultCount)
        {
            try
            {
                var ret = new List<ColaboradorAtivoDTO>();

                var colaboradoresAtivos = _colaboradorDtoRepository.BuscarColaboradoresAtivos(cursor, limite, out totalResultCount);
                filteredResultCount = colaboradoresAtivos.Count();

                foreach (var item in colaboradoresAtivos)
                {
                    ret.Add(new ColaboradorAtivoDTO
                    {
                        Cpf = item.Cpf,
                        NomeCompleto = item.NomeCompleto,
                        Email = item.Email,
                        Diretoria = item.Diretoria,
                        Status = item.Status,
                        Slack_id = item.Slack_id
                    });
                }

                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ItemPerfilResult>> InserirSoftskillColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            return await _softskillClient.InserirSoftskillColaborador(token, dtos);
        }

        public async Task<StatusResult> RemoverSoftskillColaborador(string token, string cpf, long id)
        {
            return await _softskillClient.RemoverSoftskillColaborador(token, cpf, id);
        }

        public Task<ItemPerfilResult> AlterarSoftskillColaborador(AdicionarRemoverItemDTO dtos)
        {
            return _softskillClient.AlterarSoftskillColaborador(dtos, _usuarioLogado.Token);
        }

        public Task<CertificadoDTO> InserirCertificadoCompetenciaColaborador(string cpfRequest, long competenciaColaboradorId, byte[] file, TipoCertificadoEnum tipo, DateTime dataConclusao, string instituicao, string descricao, int cargaHoraria)
        {
            return _competenciaClient.InserirCertificadoCompetenciaColaborador(cpfRequest, competenciaColaboradorId, file, tipo, _usuarioLogado.Token, dataConclusao, instituicao, descricao, cargaHoraria);
        }

        public List<TipoDependenteDTO> ListarTipoDependente()
        {
            var ret = _dependenteDtoRepository.ListarTipoDependente();
            return ret;
        }

        public async Task<UsuarioColaboradorDTO> BuscarDadosUsuario(string token)
        {
            return await _usuarioClient.ShowMe(token);
        }

        public async Task InserirContatoEmergencia(ContatoEmergenciaDTO srsCandidate, string cpf)
        {
            try
            {
                srsCandidate.Ordem = Guid.NewGuid().ToString();

                var contatos = await _contatoEmergenciaRepository.ListaContatoEmergencia(cpf);
                if (contatos.Where(x => x.Telefone == srsCandidate.Telefone || x.Nome == srsCandidate.Nome).Any())
                    throw new ValidationException("Contato de emergência não pode ter o mesmo nome ou telefone");

                _contatoEmergenciaRepository.InsereContatoEmergencia(srsCandidate, cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task AlterarContatoEmergencia(ContatoEmergenciaDTO srsCandidate, string cpf)
        {
            try
            {
                var contatos = await _contatoEmergenciaRepository.ListaContatoEmergencia(cpf);
                if (contatos.Where(x => x.Telefone == srsCandidate.Telefone || x.Nome == srsCandidate.Nome).Any())
                    throw new ValidationException("Contato de emergência não pode ter o mesmo nome ou telefone");

                _contatoEmergenciaRepository.AlteraContatoEmergencia(srsCandidate);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<ContatoEmergenciaDTO>> ListarContatoEmergencia(string cpf)
        {
            try
            {
                return await _contatoEmergenciaRepository.ListaContatoEmergencia(cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task DeletarContatoEmergencia(string contactOrder, string cpf)
        {
            try
            {
                _contatoEmergenciaRepository.DeletaContatoEmergencia(new ContatoEmergenciaDTO
                {
                    Ordem = contactOrder
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task InserirDadosPCD(EnumPCD pCD, int orgId, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf)
        {
            try
            {
                var model = new ColaboradorDTO
                {
                    Cpf = cpf,
                    Saude = new ColaboradorSaudeDTO
                    {
                        PCD = pCD,
                        GrupoDeRiscoCovid = grupoDeRisco ? (sbyte)1 : (sbyte)0,
                        CondicaoDeSaudeRelevante = descricaoCondicaoDeSaude
                    }
                };
                _colaboradorDtoRepository.InserirDadosPCD(cpf, model);
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task AlterarDadosPCD(EnumPCD pCD, int orgId, bool grupoDeRisco, string descricaoCondicaoDeSaude, string cpf)
        {
            try
            {
                var model = new ColaboradorDTO
                {
                    Cpf = cpf,
                    Saude = new ColaboradorSaudeDTO
                    {
                        PCD = pCD,
                        GrupoDeRiscoCovid = grupoDeRisco ? (sbyte)1 : (sbyte)0,
                        CondicaoDeSaudeRelevante = descricaoCondicaoDeSaude
                    }
                };
                _colaboradorDtoRepository.AlterarDadosPCD(cpf, model);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task InserirDadosDemograficos(DadosDemograficosColaboradorDTO dadosDemograficos, string cpf)
        {
            try
            {
                ValidaAcessoSimuladorRemuneracao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
                // Validações básicas
                if (dadosDemograficos.QuantidadePessoasResidencia < 1)
                    throw new ArgumentException("O número de pessoas com quem mora deve ser no mínimo 1.");

                if (dadosDemograficos.DependentesIRPF < 0)
                    throw new ArgumentException("O número de dependentes IRRF não pode ser negativo.");

                if (dadosDemograficos.PossuiConjuge && !dadosDemograficos.DataNascimentoConjuge.HasValue)
                    throw new ArgumentException("Data de nascimento do cônjuge é obrigatória quando possui cônjuge.");

                if (dadosDemograficos.PossuiFilhos)
                {
                    if (dadosDemograficos.Filhos == null || !dadosDemograficos.Filhos.Any())
                        throw new ArgumentException("Deve informar pelo menos um filho quando possui filhos.");

                    foreach (var filho in dadosDemograficos.Filhos)
                    {
                        if (!filho.DataNascimento.HasValue)
                            throw new ArgumentException("Data de nascimento é obrigatória para todos os filhos.");
                    }
                }
                else
                {
                    // Se não possui filhos, garantir que a lista está vazia
                    dadosDemograficos.Filhos = new List<FilhoColaboradorDTO>();
                }

                if (dadosDemograficos.IncluirDependentesPlanoFoursys && dadosDemograficos.QuantidadeDependentesPlanoFoursys < 1)
                    throw new ArgumentException("A quantidade de dependentes do plano Foursys deve ser no mínimo 1 quando incluir dependentes.");

                // Validação de OutrosCustos
                if (dadosDemograficos.OutrosCustos != null && dadosDemograficos.OutrosCustos.Any())
                {
                    foreach (var outroCusto in dadosDemograficos.OutrosCustos)
                    {
                        if (string.IsNullOrWhiteSpace(outroCusto.Descricao))
                            throw new ArgumentException("A descrição é obrigatória para todos os outros custos.");

                        if (outroCusto.Valor < 0)
                            throw new ArgumentException("O valor dos outros custos não pode ser negativo.");
                    }
                }
                else
                {
                    // Garantir que a lista não seja null
                    dadosDemograficos.OutrosCustos = new List<OutroCustoColaboradorDTO>();
                }

                // Verificar se já existe registro
                var dadosExistentes = await _colaboradorRepository.ObterDadosDemograficosAsync(cpf);
                if (dadosExistentes != null)
                    throw new Exception("Dados demográficos já existem para este colaborador. Use o método de alteração.");

                await _colaboradorRepository.InserirDadosDemograficosAsync(dadosDemograficos, cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task AlterarDadosDemograficos(DadosDemograficosColaboradorDTO dadosDemograficos, string cpf)
        {
            try
            {
                ValidaAcessoSimuladorRemuneracao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
                // Validações básicas
                if (dadosDemograficos.QuantidadePessoasResidencia < 0)
                    throw new ArgumentException("O número de pessoas com quem mora deve ser no mínimo 0.");

                if (dadosDemograficos.DependentesIRPF < 0)
                    throw new ArgumentException("O número de dependentes IRRF não pode ser negativo.");

                if (dadosDemograficos.PossuiConjuge && !dadosDemograficos.DataNascimentoConjuge.HasValue)
                    throw new ArgumentException("Data de nascimento do cônjuge é obrigatória quando possui cônjuge.");

                if (dadosDemograficos.PossuiFilhos)
                {
                    if (dadosDemograficos.Filhos == null || !dadosDemograficos.Filhos.Any())
                        throw new ArgumentException("Deve informar pelo menos um filho quando possui filhos.");

                    foreach (var filho in dadosDemograficos.Filhos)
                    {
                        if (!filho.DataNascimento.HasValue)
                            throw new ArgumentException("Data de nascimento é obrigatória para todos os filhos.");
                    }
                }
                else
                {
                    // Se não possui filhos, garantir que a lista está vazia
                    dadosDemograficos.Filhos = new List<FilhoColaboradorDTO>();
                }

                if (dadosDemograficos.IncluirDependentesPlanoFoursys && dadosDemograficos.QuantidadeDependentesPlanoFoursys < 1)
                    throw new ArgumentException("A quantidade de dependentes do plano Foursys deve ser no mínimo 1 quando incluir dependentes.");

                // Validação de OutrosCustos
                if (dadosDemograficos.OutrosCustos != null && dadosDemograficos.OutrosCustos.Any())
                {
                    foreach (var outroCusto in dadosDemograficos.OutrosCustos)
                    {
                        if (string.IsNullOrWhiteSpace(outroCusto.Descricao))
                            throw new ArgumentException("A descrição é obrigatória para todos os outros custos.");

                        if (outroCusto.Valor < 0)
                            throw new ArgumentException("O valor dos outros custos não pode ser negativo.");
                    }
                }
                else
                {
                    // Garantir que a lista não seja null
                    dadosDemograficos.OutrosCustos = new List<OutroCustoColaboradorDTO>();
                }

                await _colaboradorRepository.AlterarDadosDemograficosAsync(dadosDemograficos, cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<DadosDemograficosColaboradorDTO> ObterDadosDemograficos(string cpf)
        {
            try
            {
                ValidaAcessoSimuladorRemuneracao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
                return await _colaboradorRepository.ObterDadosDemograficosAsync(cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task AlterarFormularioColaborador(string nome, DateTime? data_nascimento, string cpf, string rg, string estado_civil,
            string escolaridade, string etnia, string genero, string orientacao_sexual, bool pessoa_refugiada, string email, string emailAlternativo,
            string celular, string passaporte, EnderecoDTO endereco, ColaboradorSaudeDTO saudeColab, string documentoColaborador, int orgId)
        {
            try
            {
                using (var dbTrans = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        var colaboradorBanco = _colaboradorDtoRepository.GetModelByKey(new ColaboradorDTO { Cpf = cpf });

                        var enderecoBanco = _enderecoDtoRepository.GetModelByKey(colaboradorBanco.Cpf);

                        if (enderecoBanco == null)
                        {
                            var enderecoNovo = MontaEnderecoRow(endereco);
                            _enderecoDtoRepository.SaveModel(enderecoNovo);
                        }
                        else
                        {
                            var enderecoAtualizado = MontaEnderecoRow(endereco, enderecoBanco);
                            _enderecoDtoRepository.UpdateModel(enderecoAtualizado);
                        }
                        colaboradorBanco.Saude = saudeColab;
                        colaboradorBanco.Endereco = enderecoBanco;
                        colaboradorBanco.DataNascimento = data_nascimento;
                        colaboradorBanco.Rg = rg;
                        colaboradorBanco.Passaporte = passaporte;
                        colaboradorBanco.ContatoPrincipal = celular;
                        colaboradorBanco.EstadoCivil = estado_civil;
                        colaboradorBanco.Genero = genero;
                        colaboradorBanco.Etnia = etnia;
                        colaboradorBanco.Escolaridade = escolaridade;
                        colaboradorBanco.OrientacaoSexual = orientacao_sexual;
                        colaboradorBanco.EmailAlternativo = emailAlternativo;
                        colaboradorBanco.PessoaRefugiada = pessoa_refugiada;
                        colaboradorBanco.DocumentoColaborador = documentoColaborador;

                        _colaboradorDtoRepository.UpdateModel(colaboradorBanco);

                        dbTrans.Commit();
                    }
                    catch (Exception err)
                    {
                        dbTrans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task AlterarCurriculoColaborador(byte[] file, string cpf)
        {
            try
            {
                var curriculo = file != null ? Convert.ToBase64String(file) : "";

                var relativePath =
                            VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CURRICULO) +
                            "curriculo_" +
                            cpf +
                            "_" +
                            DateTime.Now.ToString("yyyyMMddHHmmssFFF") + ".pdf";

                var basePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                var path = basePath + relativePath;

                _curriculoColaboradorRespository.AlterarCurriculoColaborador(path, cpf);

                await _uploadFilesClient.UploadFile(relativePath, file);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<string> GetCurriculoColaborador(string cpf)
        {
            try
            {
                return _curriculoColaboradorRespository.GetCurriculoColaborador(cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ApiGenericResult<string>> InserirColaborador(CadastroColaboradorInput colaborador, string cpfUsuario, int orgId)
        {
            var ret = new ApiGenericResult<string>();

            using (var dbTrans = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _colaboradorValidadorService.ValidaColaboradorCadastro(colaborador, cpfUsuario, orgId, ret);

                    await VerificarConfiguracaoGrupoAcesso(orgId, cpfUsuario, ret);

                    if (!ret.Sucesso)
                    {
                        return ret;
                    }

                    var idColab = Guid.NewGuid().ToString();
                    _usuarioColaboradorRepository.InsertUsuarioColaborador(new UsuarioColaboradorDTO
                    {
                        Email = colaborador.Email,
                        Cpf = idColab,
                        OrgId = orgId
                    }, new ColaboradorDTO
                    {
                        Cpf = idColab,
                        DocumentoColaborador = colaborador.DocumentoColaborador,
                        NomeCompleto = colaborador.NomeColaborador,
                        DataAdmissao = colaborador.DataAdmissao,
                        Email = colaborador.Email,
                        Diretoria = new DiretoriaDTO()
                        {
                            Id = colaborador.CodDiretoria,
                            Diretoria = colaborador.Diretoria
                        },
                        ContatoPrincipal = colaborador.ContatoPrincipal,
                        ContatoPrincipalDDI = colaborador.ContatoPrincipalDDI,
                        VisualizarBuscaAderencia = colaborador.ConsiderarVisualizacaoAderentes,
                        CodigoModeloContratacao = colaborador.ModeloContratacao
                    }, null);
                    
                    string cargoInput = colaborador.Cargo.ToStringOuVazio();
                    string codigoCargoInput = colaborador.CodigoCargo.ToStringOuVazio();

                    // Sempre garantem string (nunca null)
                    string cargo = cargoInput;  
                    string codigoCargo = string.IsNullOrWhiteSpace(codigoCargoInput)
                        ? cargoInput
                        : codigoCargoInput;
                    
                    if (!_usuarioColaboradorRepository.CheckColaboradorOrg(idColab, orgId))
                        _usuarioColaboradorRepository.UpsertColaboradorOrg(
                            new ColaboradorDTO
                            {
                                Cpf = idColab,
                                NomeCompleto = colaborador.NomeColaborador,
                                DataAdmissao = colaborador.DataAdmissao,
                                Email = colaborador.Email,
                                Diretoria = new DiretoriaDTO()
                                {
                                    Id = colaborador.CodDiretoria,
                                    Diretoria = colaborador.Diretoria
                                },
                                Cargo = new CargoDTO()
                                {
                                    Id = 0,
                                    Cargo = ""
                                },
                                ContatoPrincipalDDI = colaborador.ContatoPrincipalDDI,
                                ContatoPrincipal = colaborador.ContatoPrincipal,
                                CodigoModeloContratacao = colaborador.ModeloContratacao
                            },
                            new ColaboradorOrgDTO
                            {
                                OrgId = orgId,
                                CodColaborador = colaborador.CodColaborador,
                                Cpf = idColab,
                                CodDiretoria = colaborador.CodDiretoria,
                                Diretoria = colaborador.Diretoria,
                                CodDepartamento = colaborador.CodDepartamento,
                                Departamento = colaborador.Departamento,
                                Cargo = cargo,
                                CodCargo = codigoCargo,
                                DataAdmissao = colaborador.DataAdmissao,
                                ModeloContratacao = colaborador.ModeloContratacao,
                                EmpresaRelacionada = colaborador.EmpresaRelacionada,
                                ModeloTrabalho = colaborador.ModeloTrabalho,
                                DiasPorSemana = colaborador.DiasPorSemana,
                                ValorHora = colaborador.ValorHora,
                                CustoHora = colaborador.CustoHora,
                                BaseHoraMes = colaborador.BaseHoraMes,
                                Ativo = true,
                                CodigoModeloContratacao = colaborador.ModeloContratacao
                            }
                        );
                    _dBConnectionUnitOfWork.BeginTransaction();
                    if (colaborador.ConsiderarBancoDeTalentos == true)
                    {
                        await _bancoDeTalentosService.InserirBancoDeTalentos(colaborador.CodColaborador, orgId, TipoCadastroBancoDeTalentos.CADASTRO_DO_COLABORADOR.ToString());
                    }

                    await AddEmpresaRelacionadaSugestaoCasoNaoExista(colaborador, cpfUsuario, orgId);

                    // fix 5580 - previne que caso haja hierarquia pré-cadastrada (lixo de base), ela vai ser "limpa" e não vai ocorrer erro
                    _usuarioColaboradorRepository.DeletaHierarquiaColaboradorPorCodColaborador(colaborador.CodColaborador, orgId);

                    _usuarioColaboradorRepository.InsereHierarquiaColaborador(
                        new ColaboradorHierarquiaCargaDTO()
                        {
                            IdentificadorColaborador = colaborador.CodColaborador,
                            IdentificadorSuperior = colaborador.CodGestor,
                        }, orgId);



                    //Revisar: monta Link para enviar para o usuairo
                    //var ssoResult = _ssoRepository.GetCredentials(orgId);
                    var orgResult = _orgRepository.BuscarOrg(orgId);
                    var linkSSO = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE) + "/" + orgResult.Descricao.ToLower();

                    ret.Retorno = linkSSO;
                    TemplateEmailDTO retTemplateService = null;
                    string templateParametrizado = null;
                    if (orgId == 5)
                    {
                        var senha = StringUtil.GetStringAleatoria(12);
                        _usuarioColaboradorRepository.AlteraSenhaUsuario(idColab, senha, orgId);
                        retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.CADASTRO_NOVO_USUARIO_SENHA);
                        templateParametrizado = retTemplateService.Template;
                        templateParametrizado = templateParametrizado.Replace("${NOME}", colaborador.NomeColaborador);
                        templateParametrizado = templateParametrizado.Replace("${SENHA}", senha);
                        templateParametrizado = templateParametrizado.Replace("${URL}", linkSSO);
                    }
                    else if (_ssoRepository.GetCredentials(orgId) != null)
                    {
                        var UrlSSO = _buscaColaboradorRepository.MontarUrlSSO(orgId);
                        retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.CADASTRO_NOVO_USUARIO_SSO);
                        templateParametrizado = retTemplateService.Template;
                        templateParametrizado = templateParametrizado.Replace("${NOME}", colaborador.NomeColaborador);
                        templateParametrizado = templateParametrizado.Replace("${URL}", UrlSSO);
                    }
                    else
                    {
                        retTemplateService = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.CADASTRO_NOVO_USUARIO_SSO);
                        templateParametrizado = retTemplateService.Template;
                        templateParametrizado = templateParametrizado.Replace("${NOME}", colaborador.NomeColaborador);
                        templateParametrizado = templateParametrizado.Replace("${URL}", VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.URL_BASE));
                    }

                    _templateRepository.RegistraTemplateEmail(orgId, colaborador.Email, null, templateParametrizado);

                    await AssociarGrupoAcessoPorConfiguracaoAsync(orgId, cpfUsuario, idColab);

                    dbTrans.Commit();
                    _dBConnectionUnitOfWork.Commit();

                    return await Task.FromResult(ret);
                }
                catch (UnauthorizedAccessException ex)
                {
                    dbTrans.Rollback();
                    _dBConnectionUnitOfWork.Rollback();
                    throw new UnauthorizedAccessException(ex.Message);
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    _dBConnectionUnitOfWork.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        private async Task AssociarGrupoAcessoPorConfiguracaoAsync(int orgId, string cpfRequest,string idColab)
        {
            var deveAssociarGrupoAcessoPorConfiguracao = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_ASSOCIAR_GRUPO_ACESSO_POR_CONFIGURACAO, orgId, cpfRequest);

            if (deveAssociarGrupoAcessoPorConfiguracao)
            {
                await _colaboradorGrupoAcessoConfiguracao.AssociarGrupoAcessoPorConfiguracaoAsync(orgId, idColab, cpfRequest);
            }
        }

        private async Task VerificarConfiguracaoGrupoAcesso(int orgId, string cpfRequest, ApiGenericResult<string> ret)
        {
            var deveAssociarGrupoAcessoPorConfiguracao = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoEnum.DEVE_ASSOCIAR_GRUPO_ACESSO_POR_CONFIGURACAO, orgId, cpfRequest);

            if (deveAssociarGrupoAcessoPorConfiguracao)
            {
                var retVerificaConfigGrupoAcesso = await _colaboradorGrupoAcessoConfiguracao.VerificarConfiguracaoGrupoAcesso(orgId);
                if (!retVerificaConfigGrupoAcesso.Sucesso)
                {
                    ret.Sucesso = false;
                    ret.Erros = retVerificaConfigGrupoAcesso.Erros;
                }
            }
        }

        private async Task AddEmpresaRelacionadaSugestaoCasoNaoExista(CadastroColaboradorInput colaborador, string codigoInternoColaboradorCriacao, int orgId)
        {
            if (colaborador is not null
                && !colaborador.EmpresaRelacionada.IsEmpty())
            {
                var empresa = await _colaboradorSugestaoRepository.GetEmpresaPorNome(colaborador.EmpresaRelacionada, orgId);

                if (empresa.IsEmpty())
                {
                    await _colaboradorSugestaoRepository.AddEmpresaSugestao(colaborador.EmpresaRelacionada, codigoInternoColaboradorCriacao, orgId);
                }
            }
        }

        public async Task<ApiGenericResult<string>> EditarColaborador(CadastroColaboradorInput colaborador, string cpf, int orgId)
        {
            var ret = new ApiGenericResult<string>();
            try
            {
                _dBConnectionUnitOfWork.BeginTransaction();
                
                _colaboradorValidadorService.ValidaColaboradorEditar(colaborador, cpf, orgId, ret);

                await VerificarConfiguracaoGrupoAcesso(orgId, cpf, ret);
                
                if (!ret.Sucesso)
                {
                    return ret;
                }

                await _usuarioColaboradorRepository.EditarColaboradorDapperAsync(colaborador, orgId);
                var validarSeExisteBancoDeTalentos = await _bancoDeTalentosService.BuscarBancoDeTalentosPorColaborador(colaborador.CodColaborador);


                if (colaborador.ConsiderarBancoDeTalentos == true && validarSeExisteBancoDeTalentos == null)
                {
                    await _bancoDeTalentosService.InserirBancoDeTalentos(colaborador.CodColaborador, orgId, TipoCadastroBancoDeTalentos.CADASTRO_DO_COLABORADOR.ToString());
                }
                else if (colaborador.ConsiderarBancoDeTalentos == false && validarSeExisteBancoDeTalentos != null)
                {
                    await _bancoDeTalentosService.DeletarBancoDeTalentos(colaborador.CodColaborador);
                }

                await AddEmpresaRelacionadaSugestaoCasoNaoExista(colaborador, cpf, orgId);

                await AssociarGrupoAcessoPorConfiguracaoAsync(orgId, cpf, colaborador.Cpf);

                _dBConnectionUnitOfWork.Commit();

                ret.Sucesso = true;
                ret.Mensagem = "Colaborador atualizado com sucesso!";
                return ret;
            }
            catch (Exception ex)
            {
                _dBConnectionUnitOfWork.Rollback();
                throw;
            }
        }

        public async Task<string> InsereCurriculoColaborador(byte[] file, string cpf)
        {
            try
            {
                using (var dbTrans = _unitOfWork.BeginTransaction())
                {
                    if (file != null)
                    {
                        var relativePath =
                            VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PATH_CURRICULO) +
                            "curriculo_" +
                            cpf +
                            "_" +
                            DateTime.Now.ToString("yyyyMMddHHmmssFFF") + ".pdf";
                        var basePath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.SERVICE_MIDIA);
                        var path = basePath + relativePath;
                        try
                        {
                            _curriculoColaboradorRespository.InsereCurriculoColaborador(path, cpf);
                            await _uploadFilesClient.UploadFile(relativePath, file);

                            dbTrans.Commit();
                        }
                        catch (Exception)
                        {
                            dbTrans.Rollback();
                            throw;
                        }

                        return path;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<SimpleColaboradorDTO> AutoCompleteColaboradorPorDiretoria(string nome, string cpf, string diretoria, int cursor, int limite, string codGestor, int orgId)
        {
            try
            {
                return _buscaColaboradorRepository.GetColaboradoresPorDiretoria(cpf, nome?.ToUpper() ?? null, diretoria, cursor, limite, codGestor, orgId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<HoleriteSimplesDTO> BuscarHolerites(string cpf)
        {
            try
            {
                return _buscaColaboradorRepository.GetAllHolerite(cpf);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasEtnia()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;

                var ret = _estatisticasRepository.GetProcedureEstatisticasEtniaPorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasIdade()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = _estatisticasRepository.GetProcedureEstatisticasIdadePorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasTempoCasa()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = _estatisticasRepository.GetProcedureEstatisticasTempoServicoPorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasOrientacaoSexual()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = _estatisticasRepository.GetProcedureEstatisticasOrientacaoSexualPorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasEscolaridade()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = _estatisticasRepository.GetProcedureEstatisticasEscolaridadePorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<EstatisticasProcDTO>> EstatisticasGenero()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = _estatisticasRepository.GetProcedureEstatisticasGeneroPorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<EstatisticasModeloTrabalhoResult> EstatisticasModeloTrabalho()
        {
            try
            {
                var orgId = _usuarioLogado.OrgId;
                var ret = await _estatisticasRepository.GetEstatisticasModeloTrabalhoPorOrg(orgId);
                return ret;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<TotalizadoresDTO>> TotalizadoresPorUnidade()
        {
            var ret = _estatisticasRepository.ListTotalizadoresPorUnidade(_usuarioLogado.OrgId);
            return ret;
        }

        public async Task<List<ColaboradoresOrgDTO>> ListaColaboradoresOrgAsync(int orgId, int cursor, int limite, bool fourtalents, string nomeOuEmail = "", string codExterno = "", string cpfRequest = null!)
        {
            try
            {
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);

                var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                var ret = _buscaColaboradorRepository.ListaColaboradoresOrg(orgId, cursor, limite, fourtalents, nomeOuEmail, codExterno, restricaoDiretorias);
                return ret;
            }
            catch
            {
                throw;
            }
        }
        public async Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkills(bool comHardskill)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                await ValidaAcessoGrupoFuncionalidade(_usuarioLogado.Cpf, FuncionalidadeSistemaEnum.CADASTRO_PESSOAS);

                var dataTipo = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";
                var extracaoAlocacaoResult = await _buscaColaboradorRepository.RelatorioColaboradoresSkills(_usuarioLogado.OrgId, comHardskill);

                if (extracaoAlocacaoResult == null || !extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existem usuários para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);

                var fileName = comHardskill ? "Colaboradores_Com_Hardskills_" : "Colaboradores_Sem_Hardskills_";
                fileName += dataTipo;

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
                ret.Sucesso = true;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<bool> AlterarIdiomaPadrao(string idioma, string cpf, int orgId)
        {
            if (!CultureUtil.IsSupportedLanguage(idioma))
            {
                throw new ArgumentException("Idioma informado não é suportado pelo sistema.");
            }

            return await _usuarioColaboradorRepository.AlterarIdiomaPadrao(idioma, cpf, orgId);
        }

        public async Task<List<AniversariantesSemanaColaboradorDTO>> RetornaListaAniversariantesSemana(int orgId, string? codDiretoria)
        {
            var buscaParametroProximosDiasQuantidade =  _buscaParametroConfiguracaoService
                .GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.ANIVERSARIANTE_PROXIMOS_DIAS_QUANTIDADE, orgId, "");
            
            var buscaParametroOcultaEmail =  _buscaParametroConfiguracaoService
                .GetParametroConfiguracao<string>(ParametroOrgCodigoEnum.ANIVERSARIANTE_OCULTA_EMAIL, orgId, "");

            int? quantidadeDiasInt = null;
            if (buscaParametroProximosDiasQuantidade.IsNotEmpty())
            {
                quantidadeDiasInt = int.Parse(buscaParametroProximosDiasQuantidade);
            }
            
            bool? deveOcultarEmail = null;
            if (buscaParametroOcultaEmail.IsNotEmpty())
            {
                deveOcultarEmail = bool.Parse(buscaParametroOcultaEmail);
            }
            
            var listaaniversariantesSemana = await _buscaColaboradorRepository.GetColaboradoresAniversariantesDaSemana(orgId, codDiretoria, quantidadeDiasInt, deveOcultarEmail?? false);

            var tokenBase64 = _token.Base64(_aspNetUser.GetUsuarioLogado().Token);
            
            var hoje = DateTime.Today;
            
            foreach (var item in listaaniversariantesSemana)
            {
                item.UrlFoto = !string.IsNullOrEmpty(item.UrlFoto) ? item.UrlFoto.Replace("$1", tokenBase64) : item.UrlFoto;
                item.UrlFotoThumb = !string.IsNullOrEmpty(item.UrlFoto) ? item.UrlFoto.Replace("$1", tokenBase64) : item.UrlFoto;
                item.UrlFotoThumbMini = !string.IsNullOrEmpty(item.UrlFoto) ? item.UrlFoto.Replace("$1", tokenBase64) : item.UrlFoto;
                item.UrlFotoThumbVeryMini = !string.IsNullOrEmpty(item.UrlFoto) ? item.UrlFoto.Replace("$1", tokenBase64) : item.UrlFoto;
                
                if (DateTime.TryParseExact(
                        item.DataNascimento,
                        "MM/dd/yyyy HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out var dataNascimento))
                {
                    if (dataNascimento.Day == hoje.Day)
                    {
                        item.EhAniversarianteHoje = true;
                    }
                }
            }

            return listaaniversariantesSemana;
        }

        public async Task<List<DadosColaboradorDTO>> ListarDadosDoColaboradorPorIds(List<string> cpfs, int orgId, string cpfRequest)
        {
            ValidaAcessoDadosColaborador(cpfRequest, orgId, FuncionalidadeSistemaEnum.DADOS_COLABORADOR);

            var result = await _buscaColaboradorRepository.ListarDadosDoColaboradorPorIds(cpfs);
            var ret = new List<DadosColaboradorDTO>();
            foreach (var item in result)
            {
                var colaborador = new DadosColaboradorDTO();
                colaborador.Colaborador = item;
                colaborador.Escolaridades = _escolaridadeColaboradorService.ListarEscolaridadeColaborador("", 0, 500, item.Cpf);
                colaborador.PerfilProfissional = await BuscarPerfilProfissional(item.Cpf);
                ret.Add(colaborador);
            }
            return ret;
        }

        public async Task<bool> EnviarCvEmMassa(EnviarCvEmMassaParam param)
        {
            try
            {
                // Validações de acesso
                await ValidaAcessoGrupoFuncionalidade(_usuarioLogado.Cpf, FuncionalidadeSistemaEnum.CV_DIGITAL);
                var dadosUsuarioLogado = BuscarDadosColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId, _usuarioLogado.Cpf, _usuarioLogado.Token);
                var template = _templateRepository.BuscaTemplateEmail(TemplateOrgParametroEnum.CV_EM_MASSA);
                var templateFinal = template.Template;

                // Preparar os itens da tabela de colaboradores
                var tabelaItems = new StringBuilder();

                foreach (var colaborador in param.Colaboradores)
                {
                    var habilidadesCard = GerarHabilidadesCardEnvioEmail(colaborador.Habilidades);

                    var htmlContent = string.Format(@"
                        <tr>
                            <td style='padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0; font-family: Arial; font-size: 14px; font-weight: 500; color: #101828; height: 60px;'>{0}</td>
                            <td style='padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0; font-family: Arial; font-size: 14px; font-weight: 500; color: #101828; height: 60px;'>{1}</td>
                            <td style='display: flex; height: 60px; align-items: center; gap: 5px; padding-left: 12px; background-color: #ffffff; border-bottom: 1px solid #eaecf0;'>
                                <img style='width: 17px; height: 17px;' src='https://fsys2-public.s3.us-east-1.amazonaws.com/cv_link_button_icon.png'/>
                                <a href='{2}' target='_blank' style='font-family: Arial; font-size: 14px; font-weight: 500; text-align: left; text-decoration-line: underline; text-decoration-style: solid; color: #525252; cursor: pointer;'>Ver</a>
                            </td>
                        </tr>",
                        colaborador.Nome,
                        habilidadesCard,
                        colaborador.Link);

                    tabelaItems.Append(htmlContent);
                }

                var usuarioEnvioDados = $"{dadosUsuarioLogado.NomeCompleto} - {dadosUsuarioLogado.Diretoria.Diretoria}";

                templateFinal = templateFinal.Replace("${Email_Usuario_Logado}", _usuarioLogado.Email);
                templateFinal = templateFinal.Replace("${NOME_E_ORG}", usuarioEnvioDados);
                templateFinal = templateFinal.Replace("${ITENS_TABELA}", tabelaItems.ToString());

                var assuntoPadrao = "Seleção Personalizada de Profissionais para Sua Avaliação";

                if (param.Assunto.ToStringOuNull() != null)
                {
                    assuntoPadrao = param.Assunto;
                }

                foreach (var email in param.Emails)
                {
                    await _templateRepository.RegistraTemplateEmailAsync(_usuarioLogado.OrgId, email, assuntoPadrao, templateFinal);
                }

                // var caminhoArquivo = @"C:\Users\LuanKaiqueRodriguesR\Desktop\Templates\templateTeste.html"; //Gerando html na maquina para visualização nos testes
                // File.WriteAllText(caminhoArquivo, templateFinal);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GerarHabilidadesCardEnvioEmail(List<string> habilidades)
        {
            if (habilidades == null || habilidades.Count == 0)
            {
                return string.Empty;
            }

            var primeiraHabilidade = habilidades.FirstOrDefault();
            var habilidadesCard = $@"
                <span style='padding: 5px 10px; border-radius: 1000px; opacity: 0.8; background-color: #d0e2ff; font-family: Arial, sans-serif; font-size: 12px; font-weight: 500; letter-spacing: 0.3px; text-align: left;color: #0043ce;'>
                    {primeiraHabilidade}
                </span>";

            if (habilidades.Count > 1)
            {
                var cardSkillCount = $@"
                    <span style='padding: 5px 10px; border-radius: 1000px; opacity: 0.8; background-color: #d0e2ff; font-family: Arial, sans-serif; font-size: 12px; font-weight: 500; etter-spacing: 0.3px; text-align: left; color: #0043ce; margin-left: 5px;'>
                        +{habilidades.Count - 1}
                    </span>";
                habilidadesCard += cardSkillCount;
            }

            return habilidadesCard;
        }

        public async Task<List<AnalistaResponsavelDTO>> BuscarAnalistasResponsaveis(int orgId)
        {
            var result = await _buscaColaboradorRepository.BuscarAnalistasResponsaveis(orgId);
            return result;
        }

        public async Task<bool> ExisteColaboradorComEsteEmail(string email)
        {
            return await _buscaColaboradorRepository.ExisteColaboradorComEsteEmail(email);
        }

        public async Task<bool> ExisteColaboradorComEsteLinkedin(string perfilIN)
        {
            return await _buscaColaboradorRepository.ExisteColaboradorComEsteLinkedin(perfilIN);
        }

        public async Task<bool> QualificarColaborador(string cpf, string cpfUsuarioLogado)
        {
            if ((await _buscaColaboradorRepository.GetColaboradorBasicoPorCodigo(cpf)) is null)
                throw new ApplicationException("Colaborador Inexistente");

            return await _buscaColaboradorRepository.QualificarColaborador(cpf, cpfUsuarioLogado);
        }

        public async Task<bool> DesqualificarColaborador(string cpf, string cpfUsuarioLogado)
        {
            if ((await _buscaColaboradorRepository.GetColaboradorBasicoPorCodigo(cpf)) is null)
                throw new ApplicationException("Colaborador Inexistente");

            return await _buscaColaboradorRepository.DesqualificarColaborador(cpf, cpfUsuarioLogado);
        }

        public async Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorMim(string codigoInternoColaborador)
        {
            return await _buscaColaboradorRepository.GetColaboradoresQualificadosPorColaboradorQualificador(codigoInternoColaborador);
        }

        public async Task<List<ColaboradorQualificadoDTO>> GetColaboradoresQualificadosPorColaborador(string codColaboradorRecrutador)
        {
            return await _buscaColaboradorRepository.GetColaboradoresQualificadosPorColaboradorQualificador(codColaboradorRecrutador);
        }

        public bool SalvarDispositivoColaboradorApp(string codigoInternoColaborador, string deviceToken, int orgId)
        {
            try
            {
                return _usuarioColaboradorRepository.SalvarDispositivoColaboradorApp(codigoInternoColaborador, deviceToken, orgId);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void InserirDadosIA(ColaboradorIADTO colaboradorIA)
        {
            try
            {
                colaboradorIA.Id = Guid.NewGuid().ToString();
                _buscaColaboradorRepository.InserirDadosIA(colaboradorIA);
            }
            catch (Exception ex)
            {
                _log.Log($"Nao foi possivel inserir dados de retorno da IA - {JsonConvert.SerializeObject(ex.Message)}", DataTransferObject.Domain.Log.LevelsEnum.Error);
            }
        }
        
        public async Task<List<ModeloContratacaoDTO>> ListarModelosDeContratacaoPorOrg(int orgId)
        {
            return await _usuarioColaboradorRepository.ListarModelosDeContratacoesPorOrg(orgId);
        }

        public void AtualizarEnderecoColaborador(string codigoInternoColaborador, string cidade, string estado)
        {
            _usuarioColaboradorRepository.AtualizarEnderecoColaborador(codigoInternoColaborador, cidade, estado);
        }

        public async Task<IEnumerable<OrigemColaboradorDTO>> ListarOrigensColaborador()
        {
            return await _buscaColaboradorRepository.ListarOrigensColaborador();
        }

        public async Task<ApiGenericResult<List<CargoColaboradorOrgDTO>>> ListarCargosPorOrgIdAsync(int orgId, string cpfRequest)
        {
            var apiResult = new ApiGenericResult<List<CargoColaboradorOrgDTO>>();
            try
            {
                var restricaoDiretorias = await _restricaoDeAcessoService.ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(cpfRequest, orgId, RestricaoDeAcessoTipoConstants.DIRETORIA);
                var result = await _usuarioColaboradorRepository.ListarCargosPorOrgIdAsync(orgId, restricaoDiretorias);
                apiResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, "Cargos");
            }
            return apiResult;
        }

        // Métodos para edição de dados do colaborador com log
        public async Task<EditarColaboradorDTO> ObterColaboradorPorCodigoAsync(string codigoInternoColaborador)
        {
            return await _colaboradorRepository.ObterColaboradorPorCodigoAsync(codigoInternoColaborador);
        }

        public async Task<EditarColaboradorDTO> AtualizarColaboradorAsync(EditarColaboradorDTO colaborador, string codigoInternoColaboradorAlterador)
        {
            // Verificar se o colaborador existe
            var existe = await ExisteColaboradorAsync(colaborador.CodigoInternoColaborador);
            if (!existe)
            {
                throw new ApplicationException($"Colaborador com código {colaborador.CodigoInternoColaborador} não encontrado.");
            }

            // Converter o número do EnumPCD para o enum, se o front enviar o número
            if (colaborador.Saude != null)
            {
                // Se o front enviar o número do enum no campo EnumPCD, converter para o enum
                if (colaborador.Saude.EnumPCD.HasValue)
                {
                    if (Enum.IsDefined(typeof(EnumPCD), colaborador.Saude.EnumPCD.Value))
                    {
                        colaborador.Saude.PCD = (EnumPCD)colaborador.Saude.EnumPCD.Value;
                    }
                }
            }

            // Obter o estado anterior do colaborador
            var colaboradorAnterior = await ObterColaboradorPorCodigoAsync(colaborador.CodigoInternoColaborador);
            
            // Atualizar o colaborador e criar log na mesma transação
            var colaboradorAtualizado = await _colaboradorRepository.AtualizarColaboradorComLogAsync(colaborador, colaboradorAnterior, codigoInternoColaboradorAlterador);

            return colaboradorAtualizado;
        }

        public async Task<bool> ExisteColaboradorAsync(string codigoInternoColaborador)
        {
            return await _colaboradorRepository.ExisteColaboradorAsync(codigoInternoColaborador);
        }

        public async Task<List<ColaboradorLogDTO>> ListarLogsPorColaboradorAsync(string codigoInternoColaborador, int limite, int cursor)
        {
            return await _colaboradorRepository.ListarLogsPorColaboradorAsync(codigoInternoColaborador, limite, cursor);
        }

        public async Task<List<ColaboradorLogDTO>> ListarLogsPorAlteradorAsync(string codigoInternoColaboradorAlterador, int limite, int cursor)
        {
            return await _colaboradorRepository.ListarLogsPorAlteradorAsync(codigoInternoColaboradorAlterador, limite, cursor);
        }

        public async Task<List<ColaboradorLogDTO>> ListarLogsPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int limite, int cursor)
        {
            return await _colaboradorRepository.ListarLogsPorPeriodoAsync(dataInicio, dataFim, limite, cursor);
        }

        public async Task<EditarColaboradorDTO> EditarCandidaturaColaboradorComDadosDemograficos(
            EditarColaboradorInputDTO colaboradorInput,
            DadosDemograficosColaboradorDTO dadosDemograficos,
            DadosCandidaturaInputDTO dadosCandidatura,
            string codigoInternoColaboradorAlterador)
        {
            ValidaAcessoSimuladorRemuneracao(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            
            // Validações básicas
            if (colaboradorInput == null)
                throw new ArgumentException("Dados do colaborador são obrigatórios.");

            if (dadosDemograficos == null)
                throw new ArgumentException("Dados demográficos são obrigatórios.");

            if (string.IsNullOrWhiteSpace(colaboradorInput.CodigoInternoColaborador))
                throw new ArgumentException("O código interno do colaborador é obrigatório.");

            // Converter DTO de entrada para DTO interno, validando grupoDeRiscoCovid
            var colaboradorDTO = ConvertToEditarColaboradorDTO(colaboradorInput);

            // Atualizar dados do colaborador
            var colaboradorAtualizado = await AtualizarColaboradorAsync(colaboradorDTO, codigoInternoColaboradorAlterador);

            // Inserir ou atualizar dados demográficos
            var dadosExistentes = await ObterDadosDemograficos(colaboradorInput.CodigoInternoColaborador);
            if (dadosExistentes != null)
            {
                await AlterarDadosDemograficos(dadosDemograficos, colaboradorInput.CodigoInternoColaborador);
            }
            else
            {
                await InserirDadosDemograficos(dadosDemograficos, colaboradorInput.CodigoInternoColaborador);
            }

            // Atualizar dados da candidatura, se informado
            if (dadosCandidatura != null)
            {
                await AtualizarDadosCandidatura(dadosCandidatura, colaboradorInput.CodigoInternoColaborador, codigoInternoColaboradorAlterador);
            }

            return colaboradorAtualizado;
        }

        /// <summary>
        /// Converte EditarColaboradorInputDTO para EditarColaboradorDTO, validando e convertendo grupoDeRiscoCovid
        /// </summary>
        private EditarColaboradorDTO ConvertToEditarColaboradorDTO(EditarColaboradorInputDTO input)
        {
            var dto = new EditarColaboradorDTO
            {
                CodigoInternoColaborador = input.CodigoInternoColaborador,
                NomeCompleto = input.NomeCompleto,
                DataNascimento = input.DataNascimento,
                Rg = input.Rg,
                Matricula = input.Matricula,
                EnderecoId = input.EnderecoId,
                Ativo = input.Ativo,
                ContatoPrincipalDdi = input.ContatoPrincipalDdi,
                ContatoPrincipal = input.ContatoPrincipal,
                ContatoOutro = input.ContatoOutro,
                ImagemId = input.ImagemId,
                Candidato = input.Candidato,
                Passaporte = input.Passaporte,
                ColaboradorSaudeId = input.ColaboradorSaudeId,
                EstadoCivil = input.EstadoCivil,
                Genero = input.Genero,
                Etnia = input.Etnia,
                OrientacaoSexual = input.OrientacaoSexual,
                Escolaridade = input.Escolaridade,
                Refugiado = input.Refugiado,
                EmailAlternativo = input.EmailAlternativo,
                Nacionalidade = input.Nacionalidade,
                Sobre = input.Sobre,
                DocumentoColaborador = input.DocumentoColaborador,
                UrlLinkedin = input.UrlLinkedin,
                DataSyncLinkedin = input.DataSyncLinkedin,
                VisualizarBuscaAderencia = input.VisualizarBuscaAderencia,
                Qualificado = input.Qualificado,
                Endereco = input.Endereco
            };

            // Converter Saude se presente, validando grupoDeRiscoCovid
            if (input.Saude != null)
            {
                try
                {
                    // Se o front enviar o número do EnumPCD, converter para o enum
                    EnumPCD pcd = EnumPCD.Nenhuma;
                    if (input.Saude.EnumPCD.HasValue)
                    {
                        // Converter o número do enum para o enum
                        if (Enum.IsDefined(typeof(EnumPCD), input.Saude.EnumPCD.Value))
                        {
                            pcd = (EnumPCD)input.Saude.EnumPCD.Value;
                        }
                    }
                    else if (input.Saude.PCD.HasValue)
                    {
                        // Se não houver número, usar o enum diretamente (compatibilidade)
                        pcd = input.Saude.PCD.Value;
                    }

                    dto.Saude = new ColaboradorSaudeDTO
                    {
                        PCD = pcd,
                        GrupoDeRiscoCovid = input.Saude.GrupoDeRiscoCovid,
                        CondicaoDeSaudeRelevante = input.Saude.CondicaoDeSaudeRelevante
                    };
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Erro ao validar dados de saúde: {ex.Message}", ex);
                }
            }

            return dto;
        }

        /// <summary>
        /// Atualiza os dados da candidatura (modelo de trabalho, quantidade de dias presenciais e pretensão salarial)
        /// </summary>
        private async Task AtualizarDadosCandidatura(
            DadosCandidaturaInputDTO dadosCandidatura,
            string codigoInternoColaborador,
            string codigoInternoColaboradorAlterador)
        {
            // Validar se os dados obrigatórios foram informados
            if (string.IsNullOrWhiteSpace(dadosCandidatura.IdCandidatura))
            {
                throw new ArgumentException("O ID da candidatura é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(dadosCandidatura.ModeloTrabalhoId))
            {
                throw new ArgumentException("O ID do modelo de trabalho é obrigatório.");
            }

            // Buscar a candidatura específica PRIMEIRO para validar propriedade
            var candidatura = await _candidaturaRepository.ObterCandidaturaPorId(dadosCandidatura.IdCandidatura);
            
            if (candidatura == null)
            {
                throw new ApplicationException($"Candidatura não encontrada. O ID '{dadosCandidatura.IdCandidatura}' não existe na base de dados.");
            }

            // Validar se a candidatura pertence ao colaborador ANTES de validar modelo de trabalho
            if (candidatura.CodColaborador != codigoInternoColaborador)
            {
                throw new UnauthorizedAccessException("A candidatura pertence a outro colaborador.");
            }

            // Validar se o GUID do modelo de trabalho existe (após confirmar que a candidatura é do colaborador)
            if (!await _candidaturaRepository.EstaModeloTrabalhoExiste(dadosCandidatura.ModeloTrabalhoId))
            {
                throw new ArgumentException($"Modelo de trabalho não encontrado. O ID '{dadosCandidatura.ModeloTrabalhoId}' não existe na base de dados.");
            }

            // Atualizar a candidatura com o novo modelo de trabalho, quantidade de dias presenciais e pretensão salarial
            try
            {
                await _candidaturaRepository.AtualizarCandidatura(
                    dadosCandidatura.IdCandidatura,
                    dadosCandidatura.PretencaoSalarial,
                    dadosCandidatura.ModeloTrabalhoId,
                    candidatura.DisponibilidadeEntrevistaId,
                    dadosCandidatura.QuantidadeDiasPresencial,
                    codigoInternoColaboradorAlterador,
                    OrigemAlteracaoPretensaoModeloLogEnum.Simulador
                );
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao atualizar candidatura {dadosCandidatura.IdCandidatura}: {ex.Message}", DataTransferObject.Domain.Log.LevelsEnum.Error);
                throw new ApplicationException($"Erro ao atualizar candidatura. Tente novamente.");
            }
        }

        public async Task EditarModeloTrabalhoAsync(ModeloTrabalhoColaboradorDTO input, int orgId, string codigoInternoColaborador)
        {
            await _colaboradorOrgRepository.EditarModeloTrabalhoAsync(orgId, codigoInternoColaborador, input);
        }

        public async Task EditarTelefoneColaboradorAsync(string telefone, string ddi, string codigoInternoColaborador)
        {
            await _usuarioColaboradorRepository.EditarTelefoneColaborador(telefone, ddi, codigoInternoColaborador);
        }
    }
}