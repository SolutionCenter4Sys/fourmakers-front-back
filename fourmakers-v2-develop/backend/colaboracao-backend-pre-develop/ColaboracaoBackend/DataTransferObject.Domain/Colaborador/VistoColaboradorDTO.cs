using System;

namespace DataTransferObject.Domain.Colaborador
{
    public class VistoColaboradorDTO
    {
        public int Id { get; set; }

        public int IdPais { get; set; }

        public DateTime Validade { get; set; }

        public string DescricaoPais { get; set; }
    }
}