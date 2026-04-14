using Colaboracao.Helper.Util;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using Core.DomainModel;
using Core.Domain.Colaborador;
using Core.Domain.Competencia;
using Core.Domain.Formacao;
using Core.Domain.Usuario.Permissao;
using Core.DomainModel.Projeto;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Colaborador.Vistos;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Projeto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.Domain.Impl.Services
{
    [LogDomainClass]
    public class MapaCompetenciaService : IMapaCompetenciaService
    {
        private readonly ICompetenciaDtoRepository _repositoryCompetencia;
        private readonly Core.Domain.Competencia.IMapaCompetenciaRepository _mapaCompetenciaRepository;
        private readonly IFuncionalidadeSistemaRepository _funcionalidadeSistemaRepository;
        private readonly IFormacaoRepository _formacaoRepository;
        private readonly IEstatisticasRepository _estatisticasRepo;
        private readonly IClienteOrgRepository _clienteOrgRepository;

        public MapaCompetenciaService(
            ICompetenciaDtoRepository repositoryCompetencia,
            IFuncionalidadeSistemaRepository funcionalidadeSistemaRepository,
            IFormacaoRepository formacaoRepository,
            IEstatisticasRepository estatisticasRepo,
            IClienteOrgRepository clienteOrgRepository,
            IMapaCompetenciaRepository mapaCompetenciaRepository)
        {
            _repositoryCompetencia = repositoryCompetencia;
            _funcionalidadeSistemaRepository = funcionalidadeSistemaRepository;
            _formacaoRepository = formacaoRepository;
            _estatisticasRepo = estatisticasRepo;
            _clienteOrgRepository = clienteOrgRepository;
            _mapaCompetenciaRepository = mapaCompetenciaRepository;
        }

        public void ValidaTipoCompetenciaEnum(int enumValue)
        {
            if (!Enum.IsDefined(typeof(TipoCompetenciaSRSEnum), enumValue))
                throw new ArgumentException("Competência inválida.");
        }

        public Task<List<CompetenciaSumarioDTO>> ListarCompetenciaSumario(TipoCompetenciaSRSEnum competencia, int orgId)
        {
            ValidaTipoCompetenciaEnum((int)competencia);
            return _repositoryCompetencia.ListarCompetenciaSumario(competencia, orgId);
        }

        public async Task<List<FormacaoSumarioDTO>> ListarFormacaoSumario(int orgId)
        {
            return await _formacaoRepository.ListarFormacaoSumario(orgId);
        }

        public async Task<List<LocalizacaoSumarioDTO>> ListarLocalizacaoSumario(int orgId)
        {
            return await _estatisticasRepo.ListarLocalizacaoSumario(orgId);
        }
        public async Task<List<CargoColaboradorSumario>> ListarCargoSumario(int orgId)
        {
            return await _estatisticasRepo.ListarCargoSumario(orgId);
        }
        public async Task<List<ClienteSumarioDTO>> ListarClienteSumario(int orgId)
        {
            return await _clienteOrgRepository.ListarClienteSumario(orgId);
        }

        public async Task<List<EstatisticasCursoDTO>> ListarEstatisticasCursos(int orgId)
        {
            return await _estatisticasRepo.ListarEstatisticasCursos(orgId);
        }

        public async Task<List<EstatisticaCidadaniaDTO>> ListarEstatisticasCidadanias(int orgId)
        {
            return await _estatisticasRepo.ListarEstatisticasCidadania(orgId);
        }

        public async Task<List<ColaboradorSkillDetalheArrayDTO>> ListaColaboradoresSkillsDetalhes(int orgId)
        {
            var separador = "{{|}}";
            var retorno = await _mapaCompetenciaRepository.ListaColaboradoresSkillsDetalhes(orgId, separador);

            var listaResultado = retorno
                .Select(dto => new ColaboradorSkillDetalheArrayDTO(dto, separador))
                .ToList();

            return listaResultado;
        }
        public async Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkillsDetalhes(int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                var extracaoAlocacaoResult = await _mapaCompetenciaRepository.ListaColaboradoresSkillsDetalhes(orgId);

                if (!extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existem colaboradores para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);
                var fileName = "Exportacao_Colaboradores_Skills_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkillsTotalizador(int orgId)
        {
            var ret = new ApiGenericResult<FileContentResult>();
            try
            {
                var extracaoAlocacaoResult = await _mapaCompetenciaRepository.ListaColaboradoresSkillsTotalizador(orgId);

                if (!extracaoAlocacaoResult.Any())
                {
                    ret.Mensagem = "Não existem colaboradores para gerar o arquivo.";
                    ret.Sucesso = false;
                    return ret;
                }

                var fileBytes = ExcelFileUtil.CreateExcelFile(extracaoAlocacaoResult);
                var fileName = "Exportacao_Colaboradores_Skills_Totalizador_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".xlsx";

                ret.Retorno = new FileContentResult(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ret.Retorno.FileDownloadName = fileName;
            }
            catch (Exception ex)
            {
                ret.Sucesso = false;
                ret.Mensagem = ex.Message;
            }

            return ret;
        }

        public async Task<List<EstatisticaVistoDTO>> ListarEstatisticasVisto(int orgId)
        {
            return await _estatisticasRepo.ListarEstatisticasVisto(orgId);
        }
    }
}