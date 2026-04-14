using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Holerite;
using Microsoft.AspNetCore.Mvc;

namespace Financeiro.Domain.Interfaces.Holerite;

public interface IHoleriteColaboradorService
{
    Task<ApiGenericResult<IEnumerable<HoleriteColaboradorDTO>>> ListarHoleritesColaboradorPorAnoAsync(string codigoInternoColaborador, int ano, int orgId);
    Task<ApiGenericResult<HoleriteColaboradorDTO>> AssinarHoleritePorLoteId(string tbItemLoteId, string tokenUsuarioLogado, string cpfLogado, int orgId);
}