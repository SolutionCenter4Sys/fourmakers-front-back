using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorIADTO
    {
        public string Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Headline { get; set; }
        public string Summary { get; set; }
        public string LocationName { get; set; }
        public string IndustryName { get; set; }
        public string UrlLinkedin { get; set; }
        public string HardSkills { get; set; }
        public string SoftSkills { get; set; }
        public string DominioNegocios { get; set; }
        public string Metodologias { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
