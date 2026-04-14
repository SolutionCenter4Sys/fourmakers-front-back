using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ProjetoAtividadeDTO
    {
        [JsonPropertyName("id")]
        public string Cod_projeto { get; set; }

        [JsonPropertyName("nome")]
        public string Projeto { get; set; }
        public string CodigoCliente { get; set; }
        public string NomeCliente { get; set; }

        [JsonPropertyName("atividades")]
        public List<AtividadeDTO> Atividades { get; set; }
    }
}