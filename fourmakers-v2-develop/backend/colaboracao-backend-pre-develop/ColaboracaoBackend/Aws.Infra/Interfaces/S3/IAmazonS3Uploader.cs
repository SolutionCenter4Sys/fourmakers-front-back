using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace Aws.Infra.Interfaces.S3
{
    public interface IAmazonS3Uploader
    {
        Task<bool> UploadFile(Stream file, string fileName);
        Task<ListVersionsResponse> FilesList();
        Task<Stream> GetFile(string key);
        Task<bool> DeleteFile(string key);
        Task<bool> RenameFile(string keySource, string keyDestination);
    }
}