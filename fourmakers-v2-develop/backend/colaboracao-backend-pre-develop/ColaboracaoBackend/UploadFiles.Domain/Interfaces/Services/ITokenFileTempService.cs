using DataTransferObject.Domain.Arquivo.TokenFileTemp;
using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UploadFiles.Domain.Interfaces.Services
{
    public interface ITokenFileTempService
    {
        Task<ApiGenericResult<List<TokenFileTempDTO>>> ListarAsync();
        Task<ApiGenericResult<TokenFileTempDTO>> ObterAsync(string token);
        /// <param name="arquivoBase">Segmento do path após <c>arquivo/</c> (form field <c>base</c>). Opcional; vazio usa <c>temp</c>. Chave S3: <c>arquivo/{base}/{guid}_{data}{ext}</c>.</param>
        Task<ApiGenericResult<TokenFileTempDTO>> InserirComUploadAsync(Stream stream, string fileNameOriginal, long fileLength, string? arquivoBase = null);
        Task<ApiGenericResult> AtualizarAsync(string token, TokenFileTempAtualizarDTO dto);
        Task<ApiGenericResult> ExcluirAsync(string token);
    }
}
