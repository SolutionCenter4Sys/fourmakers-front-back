using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;

namespace BI.Domain.Interfaces.Services;

public interface IMapaAlocacaoBIService
{
    Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> GetAderenciaAlocados(string tokenSistema);
}