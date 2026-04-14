using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento.FolhaPonto;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

namespace Core.Domain.Financeiro.Rubrica;

public interface IRubricaCargaRepository
{
    Task<List<RubricaCargaStatusDTO>> ListarHistoricoDeCargaRecentes(int orgId, List<string>? diretorias);
    Task<RubricaCargaStatusDTO> ObterCargaPorCodigoCarga(string cargaId);
    Task<List<RubricaCargaStatusDTO>> ListarCargasPorOrg(int orgId, string codigoDiretoria, string rubricaId, int mes, int ano);
    Task<IEnumerable<RubricaCargaOrigemPdfDTO>> ObterPDFCargaPorMesEAno(int orgId, string codigoInternoColaborador, int mes, int ano);
    Task<List<RubricaCargaItemLogDTO>> ObterItensLogPorCodigoCarga(string codigoCarga);
    Task<List<LoteFilaRubricaDTO>> BuscarLotesPorOrgAsync(int orgId, TipoFilaEnum tipoFila);
    Task<TemplateRubricaDTO> ObterTemplateRubricaPorRubricaId(string rubricaId);
    Task<RubricaAnaliseColaboradorDetalhadoDTO> IdentificarColaboradorDetalhadoPorCampo(string codigo, string tipoIdentificacao, int orgId);
    Task SalvarLogItemAsync(string codigoCargaRubrica, object item, string status, string tipoIdentificacao, string? codigoInternoColaborador, string? tbRubricaColaboradorId, string mensagemErro);
    Task<string> CriarRubricaCargaLoteInicial(string pdfPath, string codDiretoria, string rubricaId, string rubricaTemplateId, string codigoInternoColaboradorCriacao, int orgId, int mesInicial, int anoInicial);
    Task AtualizarRubricaCargaLogAsync(string cargaId, string jsonFinal, string status, int registrosProcessados, int registrosProcessadosComSucesso, string mensagemErro = null);
}