using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using Core.Domain.Usuario.GestaoDeAcesso;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Org;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso;

using Logs.Infra.Attributes;

namespace Usuario.Domain.Impl.Services.GestaoDeAcesso.Recurso
{
    [LogDomainClass]
    public class RecursoOrgDisponivelService : IRecursoOrgDisponivelService
    {
        public readonly static string DESCRICAO_ENTIDADE = "Recurso Org Disponível";

        private readonly IRecursoOrgDisponivelRepository _recursoOrgDisponivelRepository;
        private readonly IDBConnectionUnitOfWork _dbConnectionUnitOfWork;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly ILogDBCore _logDB;
        private readonly IOrgRepository _orgRepository;
        private readonly IRecursoMenuRepository _recursoMenuRepository;

        public RecursoOrgDisponivelService(IRecursoOrgDisponivelRepository recursoOrgDisponivelRepository,
                                           IDBConnectionUnitOfWork dbConnectionUnitOfWork,
                                           IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                           ILogDBCore logDB,
                                           IOrgRepository orgRepository,
                                           IRecursoMenuRepository recursoMenuRepository)
        {
            _recursoOrgDisponivelRepository = recursoOrgDisponivelRepository;
            _dbConnectionUnitOfWork = dbConnectionUnitOfWork;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _logDB = logDB;
            _orgRepository = orgRepository;
            _recursoMenuRepository = recursoMenuRepository;
        }
        public async Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ConfigurarRecursoOrgDisponivel(List<RecursoOrgDisponivelDTO> listRecursoOrgDisponivelDTO, string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                if (listRecursoOrgDisponivelDTO == null || !listRecursoOrgDisponivelDTO.Any())
                {
                    throw new ApplicationException("A lista de recursos organizacionais disponíveis não pode estar vazia ou nula.");
                }

                foreach (var recurso in listRecursoOrgDisponivelDTO)
                {
                    if (recurso.OrgId <= 0 || string.IsNullOrWhiteSpace(recurso.CodigoRecursoMenu))
                    {
                        throw new ApplicationException("Todos os itens da lista devem ter OrgId válido e Código de Recurso Menu preenchido.");
                    }
                }

                ValidarOrganizacoesExistentes(listRecursoOrgDisponivelDTO);
                await ValidarCodigosRecursoMenuExistentes(listRecursoOrgDisponivelDTO);

                _dbConnectionUnitOfWork.BeginTransaction();

                var listarRecursoOrgDisponivel = await _recursoOrgDisponivelRepository.ConfigurarRecursoOrgDisponivelAsync(listRecursoOrgDisponivelDTO);

                apiGenericResult.Retorno = listarRecursoOrgDisponivel;

                _logDB.SaveLogDefaultInDatabase("Recursos x Org - Última inserção", JsonConvert.SerializeObject(apiGenericResult.Retorno), ProcessIdentifierEnum.RecursoOrgDisponivelLastInsertedData, cpfRequest);

                _dbConnectionUnitOfWork.Commit();

            }
            catch (Exception ex)
            {
                _dbConnectionUnitOfWork.SafeRollback();
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ListarRecursoOrgDisponivelAsync(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>();

            ValidaAcessoRecurso(cpfRequest, orgId);

            try
            {
                var listarRecursoOrgDisponivel = await _recursoOrgDisponivelRepository.ListarRecursoOrgDisponivelAsync();
                apiGenericResult.Retorno = listarRecursoOrgDisponivel;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;

        }

        public async Task<ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>> ObterUltimaInsercaoRecursoOrgDisponivelLog(string cpfRequest, int orgId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<RecursoOrgDisponivelDTO>>();

            try
            {
                ValidaAcessoRecurso(cpfRequest, orgId);

                var log = await _logDB.ObterUltimoLogPorProcessIdentifierAsync(ProcessIdentifierEnum.RecursoOrgDisponivelLastInsertedData);

                apiGenericResult.Retorno = LogUtil.MontarRetornoLog<IEnumerable<RecursoOrgDisponivelDTO>>(log);
                apiGenericResult.Mensagem = LogUtil.MontarMensagemLogFormatada(log);

            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        private void ValidaAcessoRecurso(string cpfRequest, int orgId)
        {
            var isValid = _funcionalidadeSistemaRepository.ValidaAcessoFuncionalidade(
                cpfRequest, orgId, FuncionalidadeSistemaEnum.GESTAO_ACESSO);

            if (!isValid.Result)
            {
                throw new UnauthorizedAccessException($"Acesso negado para {DESCRICAO_ENTIDADE}");
            }
        }

        private void ValidarOrganizacoesExistentes(List<RecursoOrgDisponivelDTO> listRecursoOrgDisponivelDTO)
        {
            var orgsInformadas = listRecursoOrgDisponivelDTO.Select(r => r.OrgId).Distinct().ToList();
            var orgsExistentes = _orgRepository.GetAllOrgsId(false).Select(o => o.Id).ToList();
            var orgsInexistentes = orgsInformadas.Where(orgId => !orgsExistentes.Contains(orgId)).ToList();

            if (orgsInexistentes.Any())
            {
                throw new ApplicationException($"As seguintes organizações não existem no banco de dados: {string.Join(", ", orgsInexistentes)}");
            }
        }

        private async Task ValidarCodigosRecursoMenuExistentes(List<RecursoOrgDisponivelDTO> listRecursoOrgDisponivelDTO)
        {
            var codigosRecursoMenuInformados = listRecursoOrgDisponivelDTO.Select(r => r.CodigoRecursoMenu).Distinct().ToList();
            var recursosMenuExistentes = await _recursoMenuRepository.ListarRecursoMenusAsync();
            var codigosRecursoMenuExistentes = recursosMenuExistentes.Select(r => r.CodigoRecursoMenu).ToList();
            var codigosRecursoMenuInexistentes = codigosRecursoMenuInformados.Where(codigo => !codigosRecursoMenuExistentes.Contains(codigo)).ToList();

            if (codigosRecursoMenuInexistentes.Any())
            {
                throw new ApplicationException($"Os seguintes códigos de recurso menu não existem no banco de dados: {string.Join(", ", codigosRecursoMenuInexistentes)}");
            }
        }

    }
}