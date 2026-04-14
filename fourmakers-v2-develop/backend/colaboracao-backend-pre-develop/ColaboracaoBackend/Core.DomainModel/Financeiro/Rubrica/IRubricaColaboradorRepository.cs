using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento;

namespace Core.Domain.Financeiro.Rubrica
{
    public interface IRubricaColaboradorRepository
    {
        Task<IEnumerable<RubricaColaboradorResult>> ListarRubricasColaboradorAsync(int orgId, List<string>? diretorias, string? codigoInternoColaborador, int? mes, int? ano, string? rubricaId, int cursor, int limite);
        Task<RubricaColaboradorResult> ObterRubricaColaboradorPorIdAsync(Guid id);
        Task<RubricaColaboradorResult> ObterRubricaColaboradorPorCodigoAsync(string codigo, int orgId);
        Task<RubricaColaboradorResult> InserirRubricaColaboradorAsync(RubricaColaboradorInput parametroRepositoryInput);
        Task<RubricaColaboradorResult> AtualizarRubricaColaboradorAsync(RubricaColaboradorInput parametroRepositoryInput);
        Task<bool> DeletarRubricaColaboradorAsync(Guid id);
        Task<DateTime?> ObterDataVigenciaPorId(Guid vigenciaInicialId);
        Task<string?> ObterColaboradorPorCodigo(Guid codigoInternoColaborador);
        Task<Guid?> ObterIdVigenciaPorMesEAno(int mes, int ano);
        Task<List<VigenciaDTO>> ListarMesEAnosLancadosPorOrgId(int orgId);
        Task<IEnumerable<RubricaColaboradorDetalhadoDTO>> ListarRubricasColaboradorDetalhadoAsync(int mes, int ano, int orgId);
        Task<bool> ValidaSePodeRefletirEmissaoDeRubricaNFColaborador(string codigoInternoColaborador, int orgId);
        Task<IEnumerable<RubricaColaboradorResult>> ListarRubricasContabeisColaboradorPorVigenciaAsync(int mes, int ano, string codigoDiretoria, int orgId);
        Task<List<RubricaColaboradorResult>> ObterRubricasColaboradorPorListaDeCodigoAsync(List<Guid> ids);

    }

}
