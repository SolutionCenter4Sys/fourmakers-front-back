using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.ItemPerfil;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Nivel;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill.Skill;
using DataTransferObject.Domain.Match;

namespace DataTransferObject.Domain.MapaDeAlocacao.Perfil
{
    public class BuscarQuantidadesPessoasMatchSkillInput
    {
        [JsonPropertyName("hard_skills")]
        public List<HabilidadeTecnica> HardSkills { get; set; }

        [JsonPropertyName("soft_skills")]
        public List<HabilidadeComportamental> SoftSkills { get; set; }

        [JsonPropertyName("metodologias")]
        public List<Match.Metodologia> Metodologias { get; set; }

        [JsonPropertyName("dominios_negocio")]
        public List<DominioNegocio> DominiosNegocio { get; set; }

        [JsonPropertyName("idiomas")]
        public List<Match.Idioma> Idiomas { get; set; }

        [JsonPropertyName("disponibilidades")]
        public List<Disponibilidade> Disponibilidades { get; set; }
    }
}
