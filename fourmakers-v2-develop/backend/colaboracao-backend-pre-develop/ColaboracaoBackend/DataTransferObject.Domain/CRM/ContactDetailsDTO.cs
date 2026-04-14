namespace DataTransferObject.Domain.CRM
{
    public class ContactDetailsDTO
    {
        public int ContactId { get; set; }
        public string ContactNo { get; set; }
        public int AccountId { get; set; }
        public string AccountNo { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Linkedin { get; set; }
    }
}