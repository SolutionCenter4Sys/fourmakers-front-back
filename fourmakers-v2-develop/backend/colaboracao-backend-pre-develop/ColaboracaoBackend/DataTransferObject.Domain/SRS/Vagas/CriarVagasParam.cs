using DataTransferObject.Domain.Vaga;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class CriarVagasParam
    {
        [JsonPropertyName("idVaga")]
        public int IdVaga { get; set; }

        [JsonPropertyName("idUsuario")]
        public string IdUsuario { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("numeroDeVagas")]
        public int NumeroDeVagas { get; set; }

        [JsonPropertyName("taxaMaximaPorHora")]
        public Decimal TaxaMaximaPorHora { get; set; }

        [JsonPropertyName("nivelDeUrgencia")]
        public string NivelDeUrgencia { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("funcao")]
        public string Funcao { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAtualizacao")]
        public DateTime DataAtualizacao { get; set; }

        [JsonPropertyName("dataPublicacao")]
        public DateTime DataPublicacao { get; set; }

        [JsonPropertyName("dataAceitacao")]
        public DateTime DataAceitacao { get; set; }

        [JsonPropertyName("tipoLocalizacao")]
        public string TipoLocalizacao { get; set; }

        [JsonPropertyName("observacaoLocalizacao")]
        public string ObservacaoLocalizacao { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("skills")]
        public List<SkillsVagasSrsDTO> Skills { get; set; }

        [JsonPropertyName("candidatos")]
        public List<CandidatoVagaDTO> Candidatos { get; set; }
        
        [JsonPropertyName("maquina_cliente")]
        public string? MaquinaCliente { get; set; }
        
        [JsonPropertyName("maquina_four")]
        public string? MaquinaFour { get; set; }
    }
}