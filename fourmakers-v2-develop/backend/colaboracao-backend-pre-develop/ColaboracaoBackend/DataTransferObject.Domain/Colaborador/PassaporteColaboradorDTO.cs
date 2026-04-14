using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class PassaporteColaboradorDTO
    {
        public int Id { get; set; }

        public int IdNacionalidade { get; set; }

        public DateTime Validade { get; set; }

        public string DescricaoNacionalidade { get; set; }
    }
}