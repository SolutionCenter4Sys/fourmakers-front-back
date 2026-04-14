using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SSO
{
    public class SSOUserDetailResult
    {
        [JsonPropertyName("businessPhones")]
        public IEnumerable<string> BusinessPhones { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("givenName")]
        public string GivenName { get; set; }
        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; }
        [JsonPropertyName("mail")]
        public string Mail { get; set; }
        [JsonPropertyName("mobilePhone")]
        public string MobilePhone { get; set; }
        [JsonPropertyName("officeLocation")]
        public string OfficeLocation { get; set; }
        [JsonPropertyName("preferredLanguage")]
        public string PreferredLanguage { get; set; }
        [JsonPropertyName("surname")]
        public string Surname { get; set; }
        [JsonPropertyName("userPrincipalName")]
        public string UserPrincipalName { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}