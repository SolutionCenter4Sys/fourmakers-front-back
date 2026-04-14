using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Marketing.Comunicacao.Grupo;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Marketing.Comunicacao.Grupo;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Grupo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketing.Domain.Impl.Comunicacao.Grupo
{
    public class ComunicacaoGrupoService : IComunicacaoGrupoService
    {
        private const string DESCRICAO_ENTIDADE = "Marketing - Comunicacao";
        private readonly IComunicacaoGrupoRepository _repository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public ComunicacaoGrupoService(
            IComunicacaoGrupoRepository repository,
            IDBConnectionUnitOfWork dbConnectionUnitOfWork,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository)
        {
            _repository = repository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<ApiGenericResult<List<ColaboradorDisponivelModeloContratacaoResumoDTO>>> ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(int orgId)
        {
            var result = new ApiGenericResult<List<ColaboradorDisponivelModeloContratacaoResumoDTO>>();
            try { result.Retorno = await _repository.ObterSugestoesModeloContratacaoColaboradoresDisponiveisAsync(orgId); }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<List<ColaboradorDisponivelDiretoriaResumoDTO>>> ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(int orgId)
        {
            var result = new ApiGenericResult<List<ColaboradorDisponivelDiretoriaResumoDTO>>();
            try { result.Retorno = await _repository.ObterSugestoesDiretoriaColaboradoresDisponiveisAsync(orgId); }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<ColaboradoresResponseDTO>> ListarColaboradoresDisponiveisParaAdicionarAsync(string filtro, int orgId, List<string> codModeloContratacao = null, List<string> codDiretoria = null)
        {
            var result = new ApiGenericResult<ColaboradoresResponseDTO>();
            try { result.Retorno = await _repository.ListarColaboradoresDisponiveisParaAdicionarAsync(filtro, orgId, codModeloContratacao, codDiretoria); }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<List<GrupoResumoDTO>>> ListarGrupoResumoAsync(int orgId)
        {
            var result = new ApiGenericResult<List<GrupoResumoDTO>>();
            try { result.Retorno = await _repository.ListarGrupoResumoAsync(orgId); }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<GrupoDetalheDTO>> ObterGrupoAsync(string grupoId, int orgId)
        {
            var result = new ApiGenericResult<GrupoDetalheDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(grupoId)) throw new ArgumentException("Id do grupo obrigatório.");
                var grupo = await _repository.ObterGrupoAsync(grupoId, orgId);
                if (grupo == null) throw new ApplicationException("Grupo não encontrado.");
                result.Retorno = grupo;
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }

        public async Task<ApiGenericResult<GrupoResumoDTO>> InserirGrupoAsync(string cpf, int orgId, InserirGrupoRequestDTO request)
        {
            var result = new ApiGenericResult<GrupoResumoDTO>();
            var temPermissao = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(cpf, orgId, FuncionalidadeSistemaEnum.GESTAO_COMUNICADOS);
            if (!temPermissao)
                throw new UnauthorizedAccessException("Acesso negado. É necessária a permissão GESTAO_COMUNICADOS para criar grupo.");

            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Nome)) throw new ArgumentException("Nome do grupo obrigatório.");
                var grupoId = await _repository.InserirGrupoAsync(orgId, request);

                result.Retorno = new GrupoResumoDTO
                {
                    Id = Guid.Parse(grupoId),
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    PermiteCriarPublicacaoOficial = request.PermiteCriarPublicacaoOficial,
                    PublicacaoOficialRequerAprovacao = request.PublicacaoOficialRequerAprovacao,
                    AprovaPublicacaoOficial = request.AprovaPublicacaoOficial,
                    PermiteCriarComunidade = request.PermiteCriarComunidade,
                    PermiteAcessarAnalytics = request.PermiteAcessarAnalytics,
                    Status = "Ativo",
                    QuantidadeParticipantes = request.CodigoInternoColaboradoresParticipantes?.Count ?? 0
                };
                result.Mensagem = "Grupo criado com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> AtualizarGrupoAsync(string grupoId, int orgId, AtualizarGrupoRequestDTO request)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                var grupoIdRequest = !string.IsNullOrWhiteSpace(grupoId) ? grupoId : request?.GrupoId.ToString();
                if (string.IsNullOrWhiteSpace(grupoIdRequest)) throw new ArgumentException("Id do grupo obrigatório para atualização.");
                var sucesso = await _repository.AtualizarGrupoAsync(grupoIdRequest, orgId, request);
                if (!sucesso) throw new ApplicationException("Grupo não encontrado para atualização.");
                result.Mensagem = "Grupo atualizado com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Update, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult> DeletarGrupoAsync(string grupoId, int orgId)
        {
            var result = new ApiGenericResult();
            _dbConnectionUnitOfWork.BeginTransaction();
            try
            {
                if (string.IsNullOrWhiteSpace(grupoId)) throw new ArgumentException("Id do grupo obrigatório para remoção.");
                var sucesso = await _repository.DeletarGrupoAsync(grupoId, orgId);
                if (!sucesso) throw new ApplicationException("Grupo não encontrado para remoção.");
                result.Mensagem = "Grupo removido com sucesso.";

                _dbConnectionUnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.Rollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }
            return result;
        }

        public async Task<ApiGenericResult<PermissoesGrupoUsuarioResponseDTO>> ObterPermissoesGruposUsuarioLogadoAsync(int orgId, string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<PermissoesGrupoUsuarioResponseDTO>();
            try
            {
                var somentePublicacaoOficialNoFeed = await _repository.ObterSomentePublicacaoOficialNoFeedOrgAsync(orgId);
                var ocultarCriadorComunidade = await _repository.ObterOcultarCriadorComunidadeOrgAsync(orgId);
                result.Retorno = await _repository.ObterPermissoesGruposUsuarioAsync(codigoInternoColaborador, orgId, somentePublicacaoOficialNoFeed, ocultarCriadorComunidade);
                result.Retorno.GestaoComunicados = await _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(codigoInternoColaborador, orgId, FuncionalidadeSistemaEnum.GESTAO_COMUNICADOS);
            }
            catch (Exception ex) { ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE); }
            return result;
        }
    }
}
