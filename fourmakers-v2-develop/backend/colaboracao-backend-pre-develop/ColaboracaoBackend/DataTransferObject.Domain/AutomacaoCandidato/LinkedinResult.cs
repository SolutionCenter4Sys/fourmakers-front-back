using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataTransferObject.Domain.AutomacaoCandidato
{
    public class LinkedinResult
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("linkedinAddress")]
        public string LinkedinAddress { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("fonte")]
        public string Fonte { get; set; }

        [JsonProperty("skillsFound")]
        public List<SkillFound> SkillsFound { get; set; }

        [JsonProperty("skillsNotFound")]
        public SkillNotFound SkillNotFound { get; set; }
    }

    public class SkillFound
    {
        [JsonProperty("idcategoria")]
        public int Idcategoria { get; set; }

        [JsonProperty("skills")]
        public List<Skill> Skills { get; set; }
    }

    public class Skill
    {
        [JsonProperty("iddescription")]
        public int Iddescription { get; set; }

        [JsonProperty("idnivel")]
        public int Idnivel { get; set; }
    }

    public class SkillNotFound
    {
        [JsonProperty("skills")]
        public List<ObjectNotFound> Skills { get; set; }

        [JsonProperty("languages")]
        public List<ObjectNotFound> languages { get; set; }
    }

    public class ObjectNotFound
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}