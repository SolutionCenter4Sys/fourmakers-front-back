using System;

namespace DataTransferObject.Domain.VagasSRS
{
    public class JobOrderSkillDTO
    {
        public int Id { get; set; }
        public int JobOrderId { get; set; }
        public int CategoryId { get; set; }
        public int DescriptionId { get; set; }
        public int NivelId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}