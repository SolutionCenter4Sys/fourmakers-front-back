using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Contratacao
{
    public class DiretorioDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public bool Leitura { get; set; }
        public bool Escrita { get; set; }
    }
}
