using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Enum;
using Competencia.Domain.Enums;
using Core.Domain.MapaAlocacao.GestaoDeAlocados;
using Core.Domain.Usuario.Permissao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.Domain.Impl.GestaoDeAlocados.GestorExternoPerfilSkill
{
    [LogDomainClass]
    public class GestorExternoPerfilSkillService : IGestorExternoPerfilSkillService
    {
        public static readonly string DESCRICAO_ENTIDADE = "Skill de Perfil";

        private readonly IGestorExternoPerfilSkillRepository _gestorExternoPerfilSkillRepository;
        private readonly IGestorExternoPerfilSkillValidatorService _gestorExternoPerfilSkillValidatorService;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;

        public GestorExternoPerfilSkillService(IGestorExternoPerfilSkillRepository gestorExternoPerfilSkillRepository,
                                    IGestorExternoPerfilSkillValidatorService gestorExternoPerfilSkillValidatorService,
                                    IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
                                    IDBConnectionUnitOfWork dbConnectionUnitOfWork)
        {
            _gestorExternoPerfilSkillRepository = gestorExternoPerfilSkillRepository;
            _gestorExternoPerfilSkillValidatorService = gestorExternoPerfilSkillValidatorService;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
        }

        public async Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> ListarSkillsPerfilGestorExternoPorId(Guid gestorExternoPerfilId)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>();

            try
            {
                var result = await _gestorExternoPerfilSkillRepository.ListarGestorExternoPerfilSkillsPorGestorExternoPerfilIdAsync(gestorExternoPerfilId);

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Read, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>> InserirGestorExternoPerfilSkill(GestorExternoPerfilSkillInput gestorExternoPerfilSkillInput, Guid gestorExternoPerfilId, string cpfRequest)
        {
            var apiGenericResult = new ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>();

            try
            {
                gestorExternoPerfilSkillInput.ConfigurarParaPersistencia(gestorExternoPerfilId, cpfRequest);

                await _gestorExternoPerfilSkillValidatorService.ValidaGestorExternoPerfilSkill(gestorExternoPerfilSkillInput, CRUDEnum.Create);

                var result = await _gestorExternoPerfilSkillRepository.InserirGestorExternoPerfilSkillAsync(gestorExternoPerfilSkillInput);

                if (result == null)
                {
                    throw new ApplicationException($"{DESCRICAO_ENTIDADE} não foi inserido ou recuperado corretamente.");
                }

                apiGenericResult.Retorno = result;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<ApiGenericResult> DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(Guid gestorExternoPerfilId)
        {
            var apiGenericResult = new ApiGenericResult();

            try
            {
                var sucesso = await _gestorExternoPerfilSkillRepository.DeletarGestorExternoPerfilSkillPorGestorExternoPerfilIdAsync(gestorExternoPerfilId);

                if (!sucesso)
                {
                    throw new ApplicationException($"{DESCRICAO_ENTIDADE} não foi excluído com sucesso.");
                }

                apiGenericResult.Sucesso = sucesso;
                apiGenericResult.Mensagem = $"{DESCRICAO_ENTIDADE} excluído com sucesso.";
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Delete, DESCRICAO_ENTIDADE);
            }

            return apiGenericResult;
        }

        public async Task<AdicionarCompetenciaDTO> InserirCompetenciaAPartirDeUmPerfil(string descricao, TipoCompetenciaSRSEnum competencia, string cpf)
        {
            try
            {
                var ret = await _gestorExternoPerfilSkillRepository.AdicionarCompetencia(descricao, competencia, cpf);
                return ret;
            }
            catch (Exception ex)
            {
                ExceptionUtil.GerenciarRetornoExcecao(ex, CRUDEnum.Create, "competência a partir de um perfil");
                return null;
            }
        }
    }
}