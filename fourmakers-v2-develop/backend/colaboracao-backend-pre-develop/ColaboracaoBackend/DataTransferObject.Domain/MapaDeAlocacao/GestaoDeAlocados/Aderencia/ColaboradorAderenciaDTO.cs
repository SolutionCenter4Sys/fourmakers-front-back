using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Match;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.Aderencia
{
    public class ColaboradorAderenciaDTO
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public string Cargo { get; set; }
        public string? ModeloTrabalhoAtual { get; set; }
        public double DisponibilidadeHoras { get; set; }
        public DateTime? DataDeDisponibilidade { get; set; }
        [JsonIgnore]
        public DateTime? DataAdmissao { get; set; }
        public CalculoAderenciaDTO CalculoAderencia
        {
            get;
            set;
        }

        public int TempoDeCasa { get; set; }
        public decimal? CustoHora { get; set; }
        public decimal? RateCard { get; set; }
        public string Origem { get; set; } = string.Empty;
        public LocalidadeDTO? Localidade { get; set; }
        public List<SkillNivelDTO> Skills { get; set; }

        [JsonIgnore]
        public List<PeriodoDTO> PeriodoDTOs { get; set; }
        public bool Interessado { get; set; }
        public List<string> Projetos { get; set; }
        public CandidatosMatchResponse RetornoMatch { get; set; }
    }

    public class PerfisAderentesDTO
    {
        [JsonPropertyName("nomeColaborador")]
        [Description("Limite de perfis aderentes à este colaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("codigoInternoColaborador")]
        [Description("Código interno colaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("listaPerfisAderentes")]
        [Description("Limite de perfis aderentes à este colaborador")]
        public List<PerfilAderenteDTO> ListaPerfisAderentes { get; set; }
    }

    public class PerfilAderenteDTO
    {
        [JsonPropertyName("idPerfil")]
        public Guid IdPerfil { get; set; }

        [JsonPropertyName("codGestorExterno")]
        public string? CodGestorExterno { get; set; }

        [JsonPropertyName("compatibilidade")]
        public double Compatibilidade { get; set; }

        public CalculoAderenciaDTO CalculoAderencia
        {
            get;
            set;
        }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("nomeGestor")]
        public string NomeGestor { get; set; }

        [JsonPropertyName("nomeProjeto")]
        public string NomeProjeto { get; set; }

        [JsonPropertyName("nomeCliente")]
        public string NomeCliente { get; set; }

        [JsonPropertyName("disponibilidade")]
        public DateTime? Disponibilidade { get; set; }

        [JsonPropertyName("horasDisponiveis")]
        public double HorasDisponiveis { get; set; }

        [JsonPropertyName("habilidades")]
        public List<SkillNivelDTO>? Habilidades { get; set; }

        [JsonPropertyName("modeloTrabalho")]
        public string ModeloTrabalho { get; set; }

        [JsonPropertyName("ufCidade")]
        public string UfCidade { get; set; }

        [JsonPropertyName("custo")]
        public decimal? Custo { get; set; }

        [JsonPropertyName("rateCard")]
        public decimal? RateCard { get; set; }
    }

    public class CalculoAderenciaDTO
    {
        public List<CalculoInformadoDTO> CalculosInformados { get; set; }
        public double TotalAderencia { get; set; }
    }

    public class CalculoInformadoDTO
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("razao")]
        public string Razao { get; set; }

        [JsonPropertyName("valor")]
        public double Valor { get; set; }

        [JsonPropertyName("valorMaximo")]
        public double ValorMaximo { get; set; }
    }
}