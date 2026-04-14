using Logs.Infra.Attributes;

namespace Usuario.API.DTOs
{
    public class LoginParam
    {
        public string cpfemail { get; set; }
        
        [LogMasked]
        public string senha { get; set; }
        
        public int orgId { get; set; }
    }
}