using DataTransferObject.Domain.VagasSRS;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaFourmakersSRSDTO
    {
        [JsonPropertyName("id_vaga")]
        public long Id_vaga { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("vagas_abertas")]
        public int? Vagas_abertas { get; set; }

        [JsonPropertyName("nivel")]
        public string Nivel { get; set; }

        [JsonPropertyName("data_abertura")]
        public DateTime? Data_abertura { get; set; }

        [JsonPropertyName("status_vaga")]
        public string Status_vaga { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("data_criacao")]
        public DateTime? Data_criacao { get; set; }

        [JsonPropertyName("data_alteracao")]
        public DateTime? Data_alteracao { get; set; }

        [JsonPropertyName("locTrabalho")]
        public string LocTrabalho { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("textoLinkedin")]
        public string TextoLinkedin { get; set; }

        [JsonPropertyName("confidencial")]
        public int? Confidencial { get; set; }

        [JsonPropertyName("tipoVaga")]
        public int? TipoVaga { get; set; }
        [JsonPropertyName("termometro")]
        public string Termometro { get; set; }

        [JsonPropertyName("ativo")]
        public sbyte? Ativo { get; set; }

        public List<SkillsVagasDTO> Skills { get; set; }
        public string GestorFoursys { get; set; }
        public string? Frequencia { get; set; } = null;
        public DateTime? Data_aprovacao { get; set; }
        public string NomeAprovador { get; set; }
        
                
        [JsonPropertyName("maquina_cliente")]
        public string? MaquinaCliente { get; set; }
        
        [JsonPropertyName("maquina_four")]
        public string? MaquinaFour { get; set; }
        
        [JsonPropertyName("notes")]
        public string Notes { get; set; }
    }
}