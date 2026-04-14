using System;
using System.Threading.Tasks;

namespace Core.Domain.Labs
{
    /// <summary>
    /// Repositório para log de chamadas ao Match Semântico (best_candidates/hyde).
    /// </summary>
    public interface ILabsLogMatchSemanticoRepository
    {
        /// <summary>
        /// Insere o log e retorna o id (GUID) para ser enviado ao front.
        /// </summary>
        Task<Guid> InserirAsync(int tbOrgId, string? codigoInternoColaborador, string? objetoRequest, string? objetoResponse);

        /// <summary>
        /// Verifica se existe registro com o id informado.
        /// </summary>
        Task<bool> ExisteAsync(Guid id);

        /// <summary>
        /// Quantidade de logs de geração (Match Semântico) por organização.
        /// </summary>
        Task<int> ContarPorOrgAsync(int tbOrgId);
    }
}
