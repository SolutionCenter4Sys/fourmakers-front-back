using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Carga
{
    public class ProjetoCargaDTO
    {
        public String IdentificadorProjeto { get; set; }
        public String Projeto { get; set; }
        public String IdentificadorCliente { get; set; }
        [JsonIgnore]
        public String IdentificadorClienteRefatorado { get; set; }
        public String Cliente { get; set; }
        public String IdentificadorFilial { get; set; }
        public String Filial { get; set; }
        public string Status { get; set; }
        public int IndentificadorStatus { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public String IdentificadorProposta { get; set; }
        public double QtdHorasPlanejada { get; set; }
        public double QtdHorasExecutadas { get; set; }
        public List<string> Propostas { get; set; }
    }
}