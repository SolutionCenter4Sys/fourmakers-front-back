using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.AreaAtuacao;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoAreaAtuacao.Permanencia;
using DataTransferObject.Domain.Projeto.GestorExterno;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestaoAlocadosValidaAcesso;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Projeto.Domain.Impl.Services
{
    [LogDomainClass]
    public class GestorExternoService : IGestorExternoService
    {
        public static readonly string DESCRICAO_ENTIDADE = "Gestor";

        private readonly IGestorExternoRepository _gestorExternoRepository;
        private readonly IUsuarioColaboradorRepository _usuarioColaboradorRepository;
        private readonly IGestorExternoValidatorService _gestorExternoValidatorService;
        private readonly IGestaoAlocadosValidarAcessoService _gestaoAlocadosValidarAcessoService;
        private readonly IAreaAtuacaoService _areaAtuacaoService;
        private readonly IColaboradorClient _colaboradorClient;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;

        public GestorExternoService(IGestorExternoRepository gestorExternoRepository,
                                    IUsuarioColaboradorRepository usuarioColaboradorRepository,
                                    IGestorExternoValidatorService gestorExternoValidatorService,
                                    IGestaoAlocadosValidarAcessoService gestaoAlocadosValidarAcessoService,
                                    IAreaAtuacaoService areaAtuacaoService,
                                    IColaboradorClient colaboradorClient,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _gestorExternoRepository = gestorExternoRepository;
            _usuarioColaboradorRepository = usuarioColaboradorRepository;
            _gestorExternoValidatorService = gestorExternoValidatorService;
            _gestaoAlocadosValidarAcessoService = gestaoAlocadosValidarAcessoService;
            _areaAtuacaoService = areaAtuacaoService;
            _colaboradorClient = colaboradorClient;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
        }

        public async Task<ApiGenericResult<IEnumerable<GestorExternoResult>>> ListarGestoresExternos(string cpfRequest, string codigoCliente, int orgId, string busca)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestorExternoResult>>();
            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _gestorExternoRepository.ListarGestoresExternosAsync(codigoCliente, orgId, busca);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<GestorExternoResult>> ObterGestorExternoPorCodigo(string codGestorExterno, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var result = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno, orgId);

                if (result == null)
                {
                    ExceptionUtil.NaoEncontrado(DESCRICAO_ENTIDADE);
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<GestorExternoResult>> InserirGestorExterno(GestorExternoInput gestorExternoInput, string cpfRequest, string tokenUsuario, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                var codigoInternoColaborador = Guid.NewGuid().ToString();
                gestorExternoInput.ConfigurarParaPersistencia(orgId, cpfRequest, codigoInternoColaborador);
                await _gestorExternoValidatorService.ValidaGestorExterno(gestorExternoInput, CRUDEnum.Create);

                _dbConnectionUnitOfWork.BeginTransaction();

                if (gestorExternoInput.CodGestorExterno.IsEmpty())
                {
                    gestorExternoInput.CodGestorExterno = await _gestorExternoRepository.GerarCodigoGestorExternoAsync(gestorExternoInput.OrgId);
                }

                await ProcessarCriacaoColaborador(gestorExternoInput, codigoInternoColaborador, tokenUsuario);

                var result = await _gestorExternoRepository.InserirGestorExternoAsync(gestorExternoInput, TipoCadastrogGestorExternoEnum.CADASTRO_MANUAL_USUARIO);

                if (result == null)
                {
                    ExceptionUtil.NaoInserido(DESCRICAO_ENTIDADE);
                }

                result = await ProcessarAreasDeAtuacaoAsync(gestorExternoInput.AreasDeAtuacao, orgId, gestorExternoInput.CodGestorExterno);

                _dbConnectionUnitOfWork.Commit();

                if (gestorExternoInput.PerfilLinkedin.IsNotEmpty())
                {
                    ProcessarSincronizacaoPerfilLinkedin(gestorExternoInput.PerfilLinkedin, codigoInternoColaborador, tokenUsuario);
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return apiGenericResult;
        }

        public async Task<ApiGenericResult<GestorExternoResult>> AtualizarGestorExterno(GestorExternoInput gestorExternoInput, string codGestorExterno, string cpfRequest, string tokenUsuario, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<GestorExternoResult>();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);
                var gestorExternoAtual = await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno.ToString(), orgId);

                var codigoInternoColaboradorAlteracao = cpfRequest;

                gestorExternoInput.ConfigurarParaPersistencia(orgId, codigoInternoColaboradorAlteracao, gestorExternoAtual.CodigoInternoColaborador, codGestorExterno);
                await _gestorExternoValidatorService.ValidaGestorExterno(gestorExternoInput, CRUDEnum.Update, codGestorExterno);

                var perfilLinkedinOld = gestorExternoAtual.PerfilLinkedin;

                _dbConnectionUnitOfWork.BeginTransaction();

                if (gestorExternoAtual.CodigoInternoColaborador.IsEmpty())
                {
                    string newGuid = Guid.NewGuid().ToString();
                    gestorExternoInput.CodigoInternoColaborador = newGuid;

                    await ProcessarCriacaoColaborador(gestorExternoInput, newGuid, tokenUsuario);
                }

                var result = await _gestorExternoRepository.AtualizarGestorExternoAsync(gestorExternoInput, codGestorExterno, TipoCadastrogGestorExternoEnum.CADASTRO_MANUAL_USUARIO);

                if (result == null)
                {
                    ExceptionUtil.NaoAtualizado(DESCRICAO_ENTIDADE);
                }

                result = await ProcessarAreasDeAtuacaoAsync(gestorExternoInput.AreasDeAtuacao, orgId, gestorExternoInput.CodGestorExterno);

                _dbConnectionUnitOfWork.Commit();

                if ((result.PerfilLinkedin != perfilLinkedinOld && result.PerfilLinkedin.IsNotEmpty())
                    || (gestorExternoAtual.CodigoInternoColaborador.IsEmpty() && result.CodigoInternoColaborador.IsNotEmpty())) //caso estivesse vazio e agora não está mais
                {
                    ProcessarSincronizacaoPerfilLinkedin(result.PerfilLinkedin, result.CodigoInternoColaborador, tokenUsuario);
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task ProcessarCriacaoColaborador(GestorExternoInput gestorExternoInput, string codigoInternoColaborador, string tokenUsuario)
        {
            try
            {
                _usuarioColaboradorRepository.InsertColaboradorSemUsuario(new ColaboradorDTO
                {
                    Cpf = codigoInternoColaborador,
                    NomeCompleto = gestorExternoInput.Nome,
                    Email = gestorExternoInput.Email,
                    ContatoPrincipal = gestorExternoInput.Telefone,
                    ContatoPrincipalDDI = "55",
                });
            }
            catch
            {
                throw new Exception("Não foi possível incluir um novo colaborador, erro ao cadastrar usuário.");
            }
        }

        private void ProcessarSincronizacaoPerfilLinkedin(string perfilLinkedin, string codigoInternoColaborador, string tokenUsuario)
        {
            // Executa a chamada de sincronização em segundo plano
            _ = Task.Run(async () =>
            {
                try
                {
                    await _colaboradorClient.SincronizarPerfilLinkedinServicoExterno(codigoInternoColaborador, perfilLinkedin, tokenUsuario);
                }
                catch
                {
                    // log de erro, não deve interromper o fluxo principal
                    Console.WriteLine("Não foi possível SincronizarPerfilLinkedinServicoExterno, porém, seguiremos com cadastro de usuário normalmente.");
                }
            });
        }

        public async Task<ApiGenericResult> DeletarGestorExterno(string codGestorExterno, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                _gestaoAlocadosValidarAcessoService.ValidaAcessoCadastroGestaoAlocados(cpfRequest, orgId);

                _dbConnectionUnitOfWork.BeginTransaction();

                var gestorExternoResult = (await ObterGestorExternoPorCodigo(codGestorExterno, cpfRequest, orgId)).Retorno;

                var gestorExternoInput = new GestorExternoInput();
                gestorExternoInput.AtualizarPropriedadesDaClasseBase(gestorExternoResult);
                gestorExternoInput.ConfigurarParaPersistencia(orgId, cpfRequest, codGestorExterno);

                await _gestorExternoValidatorService.ValidaGestorExterno(gestorExternoInput, CRUDEnum.Delete);

                var sucesso = await _gestorExternoRepository.DeletarGestorExternoAsync(codGestorExterno, cpfRequest, orgId);

                if (!sucesso)
                {
                    ExceptionUtil.NaoExcluido(DESCRICAO_ENTIDADE);
                }

                _dbConnectionUnitOfWork.Commit();

                apiGenericResult.Sucesso = sucesso;
                apiGenericResult.Mensagem = $"{DESCRICAO_ENTIDADE} excluído com sucesso.";
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private async Task<GestorExternoResult> ProcessarAreasDeAtuacaoAsync(List<GestorExternoAreaAtuacaoInput> areasDeAtuacao, int orgId, string codGestorExterno)
        {
            var listaResult = new List<AreaAtuacaoResult>();
            var listaGestorExternoAreaAtuacaoInput = new List<GestorExternoAreaAtuacaoInput>();

            if (areasDeAtuacao != null && areasDeAtuacao.Any())
            {
                listaResult = await _areaAtuacaoService.InserirAreasAtuacaoCasoNaoExista(
                    areasDeAtuacao,
                    orgId
                );

                listaGestorExternoAreaAtuacaoInput = areasDeAtuacao
                     .Select((areaEnviada, index) => new GestorExternoAreaAtuacaoInput
                     {
                         AreaDeAtuacao = new AreaAtuacaoInput { Id = listaResult[index].Id },
                         Permanencia = new PermanenciaInput { Id = areaEnviada.Permanencia?.Id }
                     })
                     .ToList();
            }

            await _gestorExternoRepository.AssociarAreasDeAtuacaoAoGestorExterno(codGestorExterno, listaGestorExternoAreaAtuacaoInput, orgId);

            return await _gestorExternoRepository.ObterGestorExternoPorCodigoAsync(codGestorExterno, orgId);
        }
    }
}