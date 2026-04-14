using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.VagasSRS;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DetalheDTO
    {
        [JsonPropertyName("id_vaga")]
        public long Id_Vaga { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("textoLinkedin")]
        public string TextoLinkedin { get; set; }

        [JsonPropertyName("modalidade")]
        public string Modalidade { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("TipoVaga")]
        public TipoVagaEnum? TipoVaga { get; set; }

        [JsonPropertyName("data_abertura")]
        public DateTime? Data_abertura { get; set; }
        public List<SkillsVagasDTO> SkillsVagasDTO { get; set; }
        public string Descricao { get; set; }
        public string? Frequencia { get; set; }
        public string Pais { get; set; }
        public string Cidade { get; set; }
    }
}