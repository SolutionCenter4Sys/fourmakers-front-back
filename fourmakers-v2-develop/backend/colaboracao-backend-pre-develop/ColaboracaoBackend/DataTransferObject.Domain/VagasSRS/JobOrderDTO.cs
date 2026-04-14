using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class JobOrderDTO
    {
        [JsonPropertyName("joborder_id")]
        public long JobOrderId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("openings")]
        public int? Openings { get; set; }

        [JsonPropertyName("senioridade")]
        public string Senioridade { get; set; }

        [JsonPropertyName("jo_dtabVaga")]
        public DateTime? JoDtabVaga { get; set; }

        [JsonPropertyName("jo_stVaga")]
        public string JoStVaga { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public DateTime? DateModified { get; set; }

        [JsonPropertyName("jo_locTrab")]
        public string joLocTrab { get; set; }

        [JsonPropertyName("state")]
        public string state { get; set; }

        [JsonPropertyName("textForLinkedin")]
        public string textForLinkedin { get; set; }

        [JsonPropertyName("confidential_job")]
        public int? confidentialJob { get; set; }
        [JsonPropertyName("tipoVaga")]
        public int? tipoVaga { get; set; }
        [JsonPropertyName("termometro")]
        public string termometro { get; set; }

        [JsonPropertyName("skills")]
        public List<SkillsVagasSRSDTO> Skills { get; set; }

        [JsonPropertyName("frequencia")] 
        public string? Frequencia { get; set; } = null;

        [JsonPropertyName("email_uniresp")]
        public string? EmailNuResp { get; set; }

        [JsonPropertyName("date_approval")]
        public DateTime? DataAprovacao { get; set; }
        
        [JsonPropertyName("maquina_cliente")]
        public int? MaquinaCliente { get; set; }
        
        [JsonPropertyName("maquina_four")]
        public int? MaquinaFour { get; set; }
        
        [JsonPropertyName("notes")]
        public string Notes { get; set; }
    }
}