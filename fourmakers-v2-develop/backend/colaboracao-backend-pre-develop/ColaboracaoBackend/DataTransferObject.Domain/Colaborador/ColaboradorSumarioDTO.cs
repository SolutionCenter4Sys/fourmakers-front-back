using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorSumarioDTO
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; }

        [JsonPropertyName("Hardskill")]
        public List<CompetenciaNivelDTO> Hardskills { get; set; }

        [JsonPropertyName("Softskill")]
        public List<SoftskillNivelDTO> Softskills { get; set; }

        [JsonPropertyName("Dominio")]
        public List<DominioNivelDTO> Dominios { get; set; }

        [JsonPropertyName("Metodologia")]
        public List<MetodologiaNivelDTO> Metodologias { get; set; }

        [JsonPropertyName("idiomas")]
        public List<IdiomaNivelDTO> Idiomas { get; set; }

        [JsonPropertyName("gestor")]
        public string Gestor { get; set; }

        [JsonPropertyName("aderencia")]
        public double Aderencia { get; set; }

        [JsonPropertyName("ultimaAtualizacao")]
        public AtualizacaoCVDTO UltimaAtualizacao { get; set; }
    }
}