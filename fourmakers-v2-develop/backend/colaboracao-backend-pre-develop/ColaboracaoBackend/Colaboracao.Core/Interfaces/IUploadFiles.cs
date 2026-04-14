using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Colaboracao.Core.Interfaces
{
    public interface IUploadFiles
    {
        Task<bool> UploadFile(Stream file, string fileName);
        Task<bool> DeleteFile(string keyName);
        Task<Stream> GetFile(string keyName);
        Task<Stream> GetFileByToken(string token, string fileName);
        Task RenameFilesAsync(IEnumerable<(string NomeAtual, string NomeNovo)> renames);
    }
}