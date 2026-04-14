using System.Text.Json.Serialization;

namespace UploadFiles.API.DTOs
{
    public class ParamUploadFile
    {
        [JsonPropertyName("keyName")]
        public string KeyName { get; set; }
    }
}