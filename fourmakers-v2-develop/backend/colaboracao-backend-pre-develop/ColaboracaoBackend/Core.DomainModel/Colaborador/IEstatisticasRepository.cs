using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Colaborador.Vistos;
using DataTransferObject.Domain.Formacao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IEstatisticasRepository
    {
        List<EstatisticasProcDTO> GetProcedureEstatisticasEtniaPorOrg(int orgId);
        List<EstatisticasProcDTO> GetProcedureEstatisticasIdadePorOrg(int orgId);
        List<EstatisticasProcDTO> GetProcedureEstatisticasTempoServicoPorOrg(int orgId);
        List<EstatisticasProcDTO> GetProcedureEstatisticasOrientacaoSexualPorOrg(int orgId);
        List<EstatisticasProcDTO> GetProcedureEstatisticasEscolaridadePorOrg(int orgId);
        List<EstatisticasProcDTO> GetProcedureEstatisticasGeneroPorOrg(int orgId);
        List<TotalizadoresDTO> ListTotalizadoresPorUnidade(int orgId);
        Task<List<LocalizacaoSumarioDTO>> ListarLocalizacaoSumario(int orgId);
        Task<List<CargoColaboradorSumario>> ListarCargoSumario(int orgId);
        Task<EstatisticasModeloTrabalhoResult> GetEstatisticasModeloTrabalhoPorOrg(int orgId);
        Task<List<EstatisticasCursoDTO>> ListarEstatisticasCursos(int orgId);
        Task<List<EstatisticaCidadaniaDTO>> ListarEstatisticasCidadania(int orgId);
        Task<List<EstatisticaVistoDTO>> ListarEstatisticasVisto(int orgId);
    }
}