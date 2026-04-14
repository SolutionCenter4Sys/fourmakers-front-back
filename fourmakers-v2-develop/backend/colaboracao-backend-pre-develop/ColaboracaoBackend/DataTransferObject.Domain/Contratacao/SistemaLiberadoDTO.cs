using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class SistemaLiberadoDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
    }
}
