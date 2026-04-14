using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroDTO
    {
        public FiltroDTO()
        {
            CompetenciaNivel = new List<FiltroCompetenciaNivelDTO>();
            SoftSkillNivel = new List<FiltroSoftSkillNivelDTO>();
            FormacaoNivel = new List<FiltroFormacaoNivelDTO>();
            MetodologiaNivel = new List<FiltroMetodologiaNivelDTO>();
            DominioNivel = new List<FiltroDominioNivelDTO>();
            ModeloreferenciaNivel = new List<FiltroModeloReferenciaNivelDTO>();
            Interesse = new List<FiltroInteresseDTO>();
            Hobby = new List<FiltroHobbyDTO>();
            Unidade = new List<FiltroUnidadeDTO>();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("competencia_nivel")]
        public List<FiltroCompetenciaNivelDTO> CompetenciaNivel { get; set; }

        [JsonPropertyName("softskill_nivel")]
        public List<FiltroSoftSkillNivelDTO> SoftSkillNivel { get; set; }

        [JsonPropertyName("formacao_nivel")]
        public List<FiltroFormacaoNivelDTO> FormacaoNivel { get; set; }

        [JsonPropertyName("metodologia_nivel")]
        public List<FiltroMetodologiaNivelDTO> MetodologiaNivel { get; set; }

        [JsonPropertyName("dominio_nivel")]
        public List<FiltroDominioNivelDTO> DominioNivel { get; set; }

        [JsonPropertyName("modeloreferencia_nivel")]
        public List<FiltroModeloReferenciaNivelDTO> ModeloreferenciaNivel { get; set; }

        [JsonPropertyName("interesses")]
        public List<FiltroInteresseDTO> Interesse { get; set; }

        [JsonPropertyName("hobbies")]
        public List<FiltroHobbyDTO> Hobby { get; set; }

        [JsonPropertyName("unidade")]
        public List<FiltroUnidadeDTO> Unidade { get; set; }

        [JsonPropertyName("status")]
        public StatusFiltroDTO Status { get; set; }

        [JsonPropertyName("perfil")]
        public PerfilFiltroDTO Perfil { get; set; }
    }
}