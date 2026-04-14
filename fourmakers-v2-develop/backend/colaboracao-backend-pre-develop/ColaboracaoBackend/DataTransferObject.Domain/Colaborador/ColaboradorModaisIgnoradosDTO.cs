using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorModaisIgnoradosDTO
    {
        public int Id { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public int TbOrgId { get; set; }
        public string Tag { get; set; }
        public DateTime? DataCriacao { get; set; }
    }
}
