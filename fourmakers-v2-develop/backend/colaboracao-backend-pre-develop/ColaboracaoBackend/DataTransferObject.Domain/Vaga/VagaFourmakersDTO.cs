using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.SRS.Vagas;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class VagaFourmakersDTO
    {
        [JsonPropertyName("idVaga")]
        public int? IdVaga { get; set; }

        [JsonPropertyName("idUsuario")]
        public string IdUsuario { get; set; }

        [JsonPropertyName("unidade")]
        public string Unidade { get; set; }

        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("numeroDeVagas")]
        public int NumeroDeVagas { get; set; }

        [JsonPropertyName("taxaMaximaPorHora")]
        public Decimal TaxaMaximaPorHora { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("descricaoCargo")]
        public string DescricaoCargo { get; set; }
        [JsonPropertyName("descricaoTecnica")]
        public string DescricaoTecnica { get; set; }
        [JsonPropertyName("acrescimo")]
        public Decimal? Acrescimo { get; set; }
        [JsonPropertyName("gestorFoursys")]
        public string GestorFoursys { get; set; }
        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }
        [JsonPropertyName("dataPrevistaInicio")]
        public DateTime DataPrevistaInicio { get; set; }

        [JsonPropertyName("dataAtualizacao")]
        public DateTime DataAtualizacao { get; set; }

        [JsonPropertyName("tipoLocalizacao")]
        public string TipoLocalizacao { get; set; }

        [JsonPropertyName("solicitante")]
        public string Solicitante { get; set; }
        [JsonPropertyName("aprovador")]
        public string Aprovador { get; set; }
        [JsonPropertyName("termometro")]
        public string Termometro { get; set; }
        [JsonPropertyName("stackPrincipal")]
        public string StackPrincipal { get; set; }
        [JsonPropertyName("configuracaoMaquina")]
        public string ConfiguracaoMaquina { get; set; }
        [JsonPropertyName("duracaoContrato")]
        public string DuracaoContrato { get; set; }

        [JsonPropertyName("duracaoContratoDeterminado")]
        public Decimal? DuracaoContratoDeterminado { get; set; }

        [JsonPropertyName("tipoContratacao")]
        public List<string> TipoContratacao { get; set; }
        [JsonPropertyName("cargaHoraria")]
        public string CargaHoraria { get; set; }

        [JsonPropertyName("frequencia")]
        public string Frequencia { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }
        [JsonPropertyName("OportunidadePropostaCRM")]
        public string OportunidadePropostaCRM { get; set; }
        [JsonPropertyName("hardskills")]
        public List<SkillNivelDTO> Hardskills { get; set; }
        [JsonPropertyName("softskills")]
        public List<SkillNivelDTO> Softskills { get; set; }
        [JsonPropertyName("metodologias")]
        public List<SkillNivelDTO> Metodologias { get; set; }
        [JsonPropertyName("dominios")]
        public List<SkillNivelDTO> Dominios { get; set; }
        [JsonPropertyName("idiomas")]
        public List<SkillNivelDTO> Idiomas { get; set; }
        [JsonPropertyName("perfilId")]
        public string PerfilId { get; set; }

        [JsonPropertyName("crmInfo")]
        public CrmInfoSRSParam CrmInfo { get; set; }
        
        [JsonPropertyName("maquina_cliente")]
        public string? MaquinaCliente { get; set; }
        
        [JsonPropertyName("maquina_four")]
        public string? MaquinaFour { get; set; }
        
        
        [JsonPropertyName("notes")]
        public  string Notes { get; set; } 
    }
}