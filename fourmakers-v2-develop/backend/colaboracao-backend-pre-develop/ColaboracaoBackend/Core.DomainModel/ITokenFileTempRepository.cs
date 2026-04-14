using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ITokenFileTempRepository
    {
        Task<IEnumerable<TokenFileTempDTO>> ListarAsync();

        /// <summary>Registros com data_criacao anterior a <paramref name="limiteUtc"/> (expurgo rotineiro).</summary>
        Task<IReadOnlyList<TokenFileTempDTO>> ListarCriadosAntesDeAsync(DateTime limiteUtc);

        Task<TokenFileTempDTO> ObterPorTokenAsync(string token);
        Task InserirAsync(string token, string nomeArquivo);
        Task<int> AtualizarAsync(string token, string nomeArquivo);
        Task<int> ExcluirAsync(string token);

        /// <summary>Remove vários tokens de uma vez (após consumo bem-sucedido na solicitação).</summary>
        Task<int> ExcluirPorTokensAsync(IEnumerable<string> tokens);
    }
}
