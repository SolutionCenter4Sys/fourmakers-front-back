namespace Usuario.API.DTOs
{
    public class GetAccessTokenParam
    {
        public string AccessCode { get; set; }
        public int OrgId { get; set; }
    }
}