using Competencia.Domain.Enums;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Colaborador.Vistos;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Projeto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.Domain.Interfaces.Services
{
    public interface IMapaCompetenciaService
    {
        Task<List<CompetenciaSumarioDTO>> ListarCompetenciaSumario(TipoCompetenciaSRSEnum competencia, int orgId);
        Task<List<FormacaoSumarioDTO>> ListarFormacaoSumario(int orgId);
        Task<List<LocalizacaoSumarioDTO>> ListarLocalizacaoSumario(int orgId);
        Task<List<CargoColaboradorSumario>> ListarCargoSumario(int orgId);
        Task<List<ClienteSumarioDTO>> ListarClienteSumario(int orgId);
        Task<List<ColaboradorSkillDetalheArrayDTO>> ListaColaboradoresSkillsDetalhes(int orgId);
        Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkillsDetalhes(int orgId);
        Task<ApiGenericResult<FileContentResult>> RelatorioColaboradoresSkillsTotalizador(int orgId);
        Task<List<EstatisticasCursoDTO>> ListarEstatisticasCursos(int orgId);
        Task<List<EstatisticaCidadaniaDTO>> ListarEstatisticasCidadanias(int orgId);
        Task<List<EstatisticaVistoDTO>> ListarEstatisticasVisto(int orgId);
    }
}