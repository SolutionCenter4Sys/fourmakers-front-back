using System;
using System.Threading.Tasks;

namespace Admissao.Domain.Interfaces.Service;

public interface IAdmissaoValidatorService
{
    /// <summary>
    /// Verifica se o pipeline existe e está ativo na organização.
    /// Lança <see cref="InvalidOperationException"/> quando não encontrado.
    /// </summary>
    Task ValidarPipelineExisteAsync(Guid pipelineId, int orgId);

    /// <summary>
    /// Verifica se o status existe e está ativo na organização.
    /// Lança <see cref="InvalidOperationException"/> quando não encontrado.
    /// </summary>
    Task ValidarStatusExisteAsync(Guid statusId, int orgId);

    /// <summary>
    /// Verifica se o colaborador existe e está ativo em <c>tb_colaborador</c>.
    /// Não faz nada quando <paramref name="codigoInternoColaborador"/> é nulo ou vazio (campo opcional).
    /// Lança <see cref="InvalidOperationException"/> quando informado mas não encontrado.
    /// </summary>
    Task ValidarColaboradorExisteAsync(string codigoInternoColaborador);
}
