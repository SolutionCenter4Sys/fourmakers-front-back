using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento;

namespace Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador
{
    public interface IRubricaColaboradorService
    {
        Task<ApiGenericResult<IEnumerable<RubricaColaboradorResult>>> ListarRubricasColaborador(string cpfRequest, int orgId, string? unidadeId, string? codigoInternoColaborador, int? mesInicial, int? anoInicial, string? rubricaId, int cursor, int limite);
        Task<ApiGenericResult<RubricaColaboradorResult>> ObterRubricaColaboradorPorId(Guid id, string CpfRequest, int orgId);
        Task<ApiGenericResult<RubricaColaboradorResult>> InserirRubricaColaborador(RubricaColaboradorInput rubricaColaboradorInput, string cpfRequest, int orgId, bool useTransaction = true,  bool validaAcesso = true);
        Task<ApiGenericResult<RubricaColaboradorResult>> AtualizarRubricaColaborador(RubricaColaboradorInput rubricaColaboradorInput, Guid id, string cpfRequest, int orgId,  bool validaAcesso = true);
        Task<ApiGenericResult> DeletarRubricaColaborador(Guid id, string cpfRequest, int orgId,  bool validaAcesso = true);
        Task<ApiGenericResult<List<VigenciaDTO>>> ListarMesEAnosLancadosPorOrgId(int orgId);
        Task<ApiGenericResult<IEnumerable<RubricaColaboradorDetalhadoDTO>>> ListarRubricasColaboradorDetalhado(string cpfRequest, int mes, int ano, int orgId);
        Task<bool> ValidaSeEPrestadorERubricaContabil(Guid rubricaId, string codigoInternoColaborador, int orgId, string cpfRequest, bool validaAcesso = true);
    }
}
