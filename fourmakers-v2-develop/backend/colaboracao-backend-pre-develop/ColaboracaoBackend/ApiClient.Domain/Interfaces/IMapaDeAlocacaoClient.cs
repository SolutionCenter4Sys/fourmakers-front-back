using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;

namespace ApiClient.Domain.Interfaces;

public interface IMapaDeAlocacaoClient
{
    Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> ListarAderenciaAlocados(string tokenSistema, int orgId);
}