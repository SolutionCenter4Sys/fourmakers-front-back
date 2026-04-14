using System;
using System.Threading.Tasks;

namespace Aws.Infra.Interfaces
{
    public interface IAwsCacheService
    {
        /// <summary>
        /// Obtém um item do cache
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser retornado</typeparam>
        /// <param name="key">Chave do item no cache</param>
        /// <returns>Objeto armazenado no cache ou null se não encontrado</returns>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Armazena um item no cache
        /// </summary>
        /// <typeparam name="T">Tipo do objeto a ser armazenado</typeparam>
        /// <param name="key">Chave para armazenar o item</param>
        /// <param name="value">Valor a ser armazenado</param>
        /// <param name="expirationTime">Tempo de expiração do item em cache (opcional)</param>
        /// <returns>True se a operação foi bem sucedida</returns>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expirationTime = null) where T : class;

        /// <summary>
        /// Remove um item do cache
        /// </summary>
        /// <param name="key">Chave do item a ser removido</param>
        /// <returns>True se o item foi removido com sucesso</returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Verifica se um item existe no cache
        /// </summary>
        /// <param name="key">Chave do item a ser verificado</param>
        /// <returns>True se o item existe no cache</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Obtém ou cria um item no cache
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="key">Chave do item</param>
        /// <param name="factory">Função para criar o item caso não exista no cache</param>
        /// <param name="expirationTime">Tempo de expiração do item em cache (opcional)</param>
        /// <returns>Objeto do cache ou recém criado</returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null) where T : class;

        /// <summary>
        /// Atualiza o tempo de expiração de um item no cache
        /// </summary>
        /// <param name="key">Chave do item</param>
        /// <param name="expirationTime">Novo tempo de expiração</param>
        /// <returns>True se a operação foi bem sucedida</returns>
        Task<bool> UpdateExpirationAsync(string key, TimeSpan expirationTime);
    }
}