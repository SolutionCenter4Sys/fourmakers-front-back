using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

namespace Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;

public interface IRubricaCargaService
{
    Task<ApiGenericResult<RubricaCargaStatusDTO>> InserirCarga(RubricaCargaInput input, string cpfRequest, string token, int orgId);
    Task<ApiGenericResult<RubricaCargaConsultaResult>> ObterStatusCargaPorId(string cargaRubricaId);
    Task<ApiGenericResult<List<RubricaCargaStatusDTO>>> ListarHistoricoDeCargaRecentes(int orgId, string cpfRequest);
    Task<ApiGenericResult<List<RubricaCargaStatusDTO>>> ListarCargasPorOrg(int orgId, string codigoDiretoria, string rubricaId, int mes, int ano);
    Task<ApiGenericResult<RubricaCargaStatusDTO>> ObterCargaPorCodigoCarga(string codigoCarga);
    Task<ApiGenericResult<IEnumerable<RubricaCargaOrigemPdfDTO>>> ObterPDFCargaPorMesEAno(int orgId, string codigoInternoColaborador, int mes, int ano);
    Task<bool> ProcessarItemRubricaCargaAsync(FilaMessageRubricaCargaDTO mensagem, string codigoColaboradorRequest, int orgId, string itemLoteId, string loteId);
    Task<ApiGenericResult<RubricaCargaStatusDTO>> CriarSumarioRubricaCarga(RubricaCargaInput input, string codigoInternoColaborador, int orgId);
}