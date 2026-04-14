namespace DataTransferObject.Domain.AutomacaoCandidato
{
    public class PowerAutomateRequest
    {
        public string Uri { get; set; }
        public string CliendId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }

    public class PowerAutomateLindelnDataRequest
    {
        public string Uri { get; set; }
        public string LinkedinAddress { get; set; }
        public string EmailAddress { get; set; }
    }
}