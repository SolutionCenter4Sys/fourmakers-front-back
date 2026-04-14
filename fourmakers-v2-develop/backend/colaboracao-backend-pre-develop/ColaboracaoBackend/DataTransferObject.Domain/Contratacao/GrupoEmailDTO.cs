using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class GrupoEmailDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
    }
}
