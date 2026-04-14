using DataTransferObject.Domain.Base;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IUploadFilesClient
    {
        Task<StatusResult> UploadFile(string fileName, byte[] arquivo);
        Task<StatusResult> DeleteFile(string keyName);

    }
}