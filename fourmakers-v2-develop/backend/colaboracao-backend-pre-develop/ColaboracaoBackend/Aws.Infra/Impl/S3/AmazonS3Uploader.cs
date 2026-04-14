using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ApiClient.Domain;
using Aws.Infra.Interfaces.S3;
using Colaboracao.Helper;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Aws.Infra.Impl.S3
{
    public class AmazonS3Uploader : IAmazonS3Uploader
    {
        private static string bucketName;
        private readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private readonly IAmazonS3 _client;

        public AmazonS3Uploader(IAmazonS3 client)
        {
            bucketName = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AWS_BUCKET_NAME);
            _client = client;
        }

        public async Task<bool> UploadFile(Stream file, string fileName)
        {
            try
            {
                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = file,
                    BucketName = bucketName,
                    Key = fileName
                };

                PutObjectResponse response = await _client.PutObjectAsync(request);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (AmazonS3Exception e)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ListVersionsResponse> FilesList()
        {
            //_client.ListObjectsV2Async(new ListObjectsV2Request() {})
            return await _client.ListVersionsAsync(bucketName);
        }
        public async Task<Stream> GetFile(string key)
        {
            GetObjectResponse response = await _client.GetObjectAsync(bucketName, key);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                return response.ResponseStream;
            else
                return null;
        }

        public async Task<bool> DeleteFile(string key)
        {
            try
            {
                DeleteObjectResponse response = await _client.DeleteObjectAsync(bucketName, key);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                    return true;
                else
                    return false;
            }
            catch (AmazonS3Exception ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RenameFile(string keySource, string keyDestination)
        {
            try
            {
                var copyRequest = new CopyObjectRequest
                {
                    SourceBucket = bucketName,
                    SourceKey = keySource,
                    DestinationBucket = bucketName,
                    DestinationKey = keyDestination
                };
                var copyResponse = await _client.CopyObjectAsync(copyRequest);
                if (copyResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    return false;

                var deleteResponse = await _client.DeleteObjectAsync(bucketName, keySource);
                return deleteResponse.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}