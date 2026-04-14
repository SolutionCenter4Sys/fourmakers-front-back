using System;
using DataTransferObject.Domain.VagasSRS;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaAnonymousDTO
    {
        [JsonIgnore]
        public long Codigo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Cargo { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string Localizacao { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public List<VagaSkillAnonymousRecrutamentoDTO> Skills { get; set; }
    }
} 