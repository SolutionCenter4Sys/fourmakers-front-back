
#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaboracao.Core.Interfaces;

public interface IRestricaoDeAcessoService
{
    Task<List<string>?> ListarMinhasRestricoesDeAcessoPorCodigoInternoETipo(string? codigoInternoColaborador, int? orgId, string restricaoTipo, string? valorPersonalizado = null);
    Task<List<string>?> ListarMinhasRestricoesDeAcessoPorCodigoInternoETipoComListaDeValores(string? codigoInternoColaborador, int orgId, string restricaoTipo, List<string>? valoresPersonalizados = null);
}