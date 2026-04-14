using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.VagasSRS;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSDTO
    {
        [JsonPropertyName("id_vaga")]
        public long Id_vaga { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("vagas_abertas")]
        public long Vagas_abertas { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("data_abertura")]
        public DateTime? Data_abertura { get; set; }

        [JsonPropertyName("status_vaga")]
        public string Status_vaga { get; set; }

        [JsonPropertyName("textoLinkedin")]
        public string TextoLinkedin { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }
        [JsonIgnore]
        [JsonPropertyName("data_criacao")]
        public DateTime? Data_criacao { get; set; }
        [JsonIgnore]
        [JsonPropertyName("data_alteracao")]
        public DateTime? Data_alteracao { get; set; }

        [JsonPropertyName("modalidade")]
        public string Modalidade { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("tipoVaga")]
        public TipoVagaEnum? TipoVaga { get; set; }

        [JsonIgnore]
        [JsonPropertyName("ativo")]
        public sbyte? Ativo { get; set; }
        [JsonPropertyName("Skills")]
        public List<SkillsVagasDTO> SkillsVagasDTO { get; set; }
        public string Descricao { get; set; }
        public string? Frequencia { get; set; }
        public bool NovaEstruturaRecrutamento { get; set; }
        public int? CodigoVagaRecrutamento { get; set; }
        public string Tracking { get; set; }
        public string Pais { get; set; }
        public string Cidade { get; set; }
        public int? OrgId { get; set; }
    }
}