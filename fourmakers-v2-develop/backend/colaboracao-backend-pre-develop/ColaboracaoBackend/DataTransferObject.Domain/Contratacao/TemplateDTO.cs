using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataTransferObject.Domain.Endereco;
using DataTransferObject.Domain.Colaborador;

namespace DataTransferObject.Domain.Contratacao
{
    public class TemplateDTO
    {
        public Guid Id { get; set; }
        public string ColaboradorCodigoInternoColaboradorAnalista { get; set; }
        public string NomeColaboradorAnalista { get; set; }
        public string ColaboradorCodigoInternoColaborador { get; set; }
        public string CandidatoVagaId { get; set; }
        public string Cargo { get; set; }
        public string EquipamentoPadraoCargoFuncaoId { get; set; }
        public string GrupoAreaEquipamentoPadraoCargoFuncao { get; set; }
        public DateTime? DataInicio { get; set; }
        public string HorarioJornada { get; set; }
        public string TipoHorarioJornada { get; set; }
        public string DocumentoColaborador { get; set; }
        public string RgColaborador { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string ContatoPrincipal { get; set; }
        public string NomeCompleto { get; set; }
        public string TamanhoCamiseta { get; set; }
        public string? DescricaoMaquina { get; set; }
        public string Hardware { get; set; }
        public string SoftwaresNecessarios { get; set; }
        public string SoftwaresEc { get; set; }
        public string? ColaboradorCodigoInternoColaboradorSuperiorImediato { get; set; }
        public string? NomeColaboradorSuperiorImediato { get; set; }
        public string EmailPessoal { get; set; }
        public string EmailCorporativo { get; set; }
        public string LoginRede { get; set; }
        public string TipoLoginRede { get; set; }
        public string? TipoMaquina { get; set; }
        public string? ObservacoesAcessoUsuario { get; set; }
        public string GrupoEmailContrato { get; set; }
        public List<OutrosGruposDTO>? OutrosGrupos { get; set; }
        public List<SistemaLiberadoDTO>? SistemasLiberados { get; set; }
        public List<DiretorioDTO>? Diretorios { get; set; }
        public List<GrupoEmailDTO>? GruposEmails { get; set; }
        public string ObservacoesAprovadorAcessos { get; set; }
        public EnderecoDTO? Endereco { get; set; }
        public decimal? Salario { get; set; }
        public decimal? CustoHora { get; set; }
        public decimal? VR { get; set; }
        public decimal? VA { get; set; }
        public decimal? AssistenciaMedica { get; set; }
        public decimal? AjudaDeCusto { get; set; }
        public decimal? Mobilidade { get; set; }
        public decimal? Educacao { get; set; }
        public decimal? RemuneracaoTotal { get; set; }
        
        [JsonPropertyName("celular")]
        public bool Celular { get; set; }
        
        [JsonPropertyName("planoDados")]
        public bool PlanoDados { get; set; }
        
        [JsonPropertyName("quantidadeMinutosPlanoDados")]
        public double QuantidadeMinutosPlanoDados { get; set; }
        
        [JsonPropertyName("cartaoVisitas")]
        public bool CartaoVisitas { get; set; }
        
        [JsonPropertyName("quantidadeCartaoVisitas")]
        public int QuantidadeCartaoVisitas { get; set; }
        
        [JsonPropertyName("outrosEquipamentos")]
        public string OutrosEquipamentos { get; set; }
        
        [JsonPropertyName("codigoVaga")]
        public int? CodigoVaga { get; set; }
        
        [JsonPropertyName("tituloVaga")]
        public string TituloVaga { get; set; }
        
        [JsonPropertyName("nomeClienteVaga")]
        public string NomeClienteVaga { get; set; }
        
        [JsonPropertyName("descricaoTipoVaga")]
        public string DescricaoTipoVaga { get; set; }
        
        [JsonPropertyName("primeiraOpcaoEquipamentoPadraoCargoFuncao")]
        public string PrimeiraOpcaoEquipamentoPadraoCargoFuncao { get; set; }
        
        [JsonPropertyName("segundaOpcaoEquipamentoPadraoCargoFuncao")]
        public string SegundaOpcaoEquipamentoPadraoCargoFuncao { get; set; }

        [JsonPropertyName("modeloTrabalhoId")]
        public string ModeloTrabalhoId { get; set; }

        [JsonPropertyName("modeloTrabalhoDescricao")]
        public string ModeloTrabalhoDescricao { get; set; }

        [JsonPropertyName("quantidadeDiasPresencial")]
        public int? QuantidadeDiasPresencial { get; set; }

        [JsonPropertyName("cargoConfianca")]
        public bool CargoConfianca { get; set; }

        [JsonPropertyName("valorAdicionalCargoConfianca")]
        public decimal? ValorAdicionalCargoConfianca { get; set; }

        [JsonPropertyName("exColaborador")]
        public bool ExColaborador { get; set; }

        [JsonPropertyName("saude")]
        public TemplateSaudeDTO Saude { get; set; }
    }
}
