using System;

namespace DataTransferObject.Domain.SSO
{
    public class SSOCredentialsDTO
    {
        public String BaseUrl { get; set; }
        public String TokenPath { get; set; }
        public String GraphPath { get; set; }
        public String Tenant { get; set; }
        public String ClientId { get; set; }
        public String ClientSecretValue { get; set; }
        public String CodeVerifierPlain { get; set; }
        public String RedirectUrl { get; set; }
        public String Scope { get; set; }
        public bool IsPublic { get; set; }
    }
}