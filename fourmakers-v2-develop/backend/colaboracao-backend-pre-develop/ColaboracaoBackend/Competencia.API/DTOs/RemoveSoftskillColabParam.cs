using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    //public class RemoveSoftskillColaboradorDTO
    //{
    //    [JsonPropertyName("softskillId")]
    //    public long SoftskillId { get; set; }

    //    [JsonPropertyName("cpf")]
    //    public string Cpf { get; set; }
    //}
    public class RemoveSoftskillColabParam
    {
        [JsonPropertyName("id")]
        public long SoftSkillId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
    }
}