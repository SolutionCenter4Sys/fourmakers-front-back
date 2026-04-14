using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.Restricao;

public interface IRestricaoDeAcessoRepository
{
    Task<List<string>> ListarRestricoesPorCodigoInternoETipo(string codigoInternoColaborador, int? orgId, string restricaoTipo);
}