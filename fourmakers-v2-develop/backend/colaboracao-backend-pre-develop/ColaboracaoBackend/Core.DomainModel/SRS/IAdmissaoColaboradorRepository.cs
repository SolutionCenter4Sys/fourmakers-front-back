using System.Threading.Tasks;

namespace Core.Domain.SRS;

public interface IAdmissaoColaboradorRepository
{
    /// <summary>
    /// Verifica se o colaborador existe e está ativo (<c>ativo = 1</c>) em <c>tb_colaborador</c>.
    /// </summary>
    Task<bool> ExisteAsync(string codigoInternoColaborador);
}
