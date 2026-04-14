using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTransferObject.Domain.Curriculo
{
    public class CurriculoXanoResponseDTO
    {
        [JsonPropertyName("emails")]
        public List<string> Emails { get; set; }
        [JsonPropertyName("colaboradores")]
        public List<ColaboradorCurriculoDTO> Colaboradores { get; set; }
        [JsonPropertyName("assunto")]
        public string Assunto { get; set; }
    }

    public class ColaboradorCurriculoDTO
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }
        [JsonPropertyName("habilidades")]
        public List<string> Habilidades { get; set; }
        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
