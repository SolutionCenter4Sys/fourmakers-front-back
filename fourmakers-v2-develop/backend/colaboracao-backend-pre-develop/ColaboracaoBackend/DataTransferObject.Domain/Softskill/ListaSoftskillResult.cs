using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Softskill
{
    public class ListaSoftskillResult : StatusResult
    {
        public ListaSoftskillResult()
        {
            SoftSkill = new List<ItemPerfilDTO>();
        }

        [JsonPropertyName("softskill")]
        public List<ItemPerfilDTO> SoftSkill { get; set; }

        public List<NivelDTO> Nivel { get; set; }
    }
}