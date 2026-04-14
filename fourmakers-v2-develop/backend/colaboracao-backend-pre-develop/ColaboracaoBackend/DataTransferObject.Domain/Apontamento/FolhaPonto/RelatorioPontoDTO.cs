using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.Helpers;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class RelatorioPontoRootDTO
    {
        [JsonPropertyName("relatorioPonto")]
        public RelatorioPontoDTO RelatorioPonto { get; set; }
    }

    public class RelatorioPontoDTO
    {
        [JsonPropertyName("empresa")]
        public EmpresaDTO Empresa { get; set; }

        [JsonPropertyName("funcionario")]
        public FuncionarioDTO Funcionario { get; set; }

        [JsonPropertyName("registrosDiarios")]
        public List<RegistroDiarioDTO> RegistrosDiarios { get; set; }

        [JsonPropertyName("resumo")]
        public ResumoDTO Resumo { get; set; }

        [JsonPropertyName("totais")]
        public TotaisDTO Totais { get; set; }
    }

    public class EmpresaDTO
    {
        [JsonPropertyName("atividadeEconomica")]
        public string AtividadeEconomica { get; set; }

        [JsonPropertyName("cnpjCpf")]
        public string CnpjCpf { get; set; }

        [JsonPropertyName("endereco")]
        public string Endereco { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }
    }

    public class FuncionarioDTO
    {
        [JsonPropertyName("baseHoras")]
        public string BaseHoras { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("carteiraDeTrabalho")]
        public string CarteiraDeTrabalho { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("dataAdmissao")]
        public string DataAdmissao { get; set; }

        [JsonPropertyName("estruturaOrganizacional")]
        public string EstruturaOrganizacional { get; set; }

        [JsonPropertyName("matricula")]
        public string Matricula { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("pis")]
        public string Pis { get; set; }
    }

    public class RegistroDiarioDTO
    {
        [JsonPropertyName("bancoDeHoras")]
        public BancoDeHorasDTO BancoDeHoras { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("descontos")]
        public string Descontos { get; set; }

        [JsonPropertyName("horarioContratual")]
        public string HorarioContratual { get; set; }

        [JsonPropertyName("justificativa")]
        public string Justificativa { get; set; }

        [JsonPropertyName("marcacoes")]
        public string Marcacoes { get; set; }

        [JsonPropertyName("rendimento")]
        public RendimentoDTO Rendimento { get; set; }
    }

    public class BancoDeHorasDTO
    {
        [JsonPropertyName("credito")]
        public string Credito { get; set; }

        [JsonPropertyName("debito")]
        public string Debito { get; set; }
    }

    public class RendimentoDTO
    {
        [JsonPropertyName("adicionalNoturno")]
        public string AdicionalNoturno { get; set; }

        [JsonPropertyName("hAtraso")]
        public string HAtraso { get; set; }

        [JsonPropertyName("hExtra")]
        public string HExtra { get; set; }

        [JsonPropertyName("hFalta")]
        public string HFalta { get; set; }

        [JsonPropertyName("hTrab")]
        public string HTrab { get; set; }
    }

    public class ResumoDTO
    {
        [JsonPropertyName("adiantamentoFechamentoBH")]
        public string AdiantamentoFechamentoBH { get; set; }

        [JsonPropertyName("atrasos")]
        public string Atrasos { get; set; }

        [JsonPropertyName("diasTrabalhados")]
        public string DiasTrabalhados { get; set; }

        [JsonPropertyName("dsr")]
        public string Dsr { get; set; }

        [JsonPropertyName("faltasDias")]
        public string FaltasDias { get; set; }

        [JsonPropertyName("heInterv")]
        public string HeInterv { get; set; }

        [JsonPropertyName("horasExtras")]
        [Newtonsoft.Json.JsonProperty("horasExtras")]
        [Newtonsoft.Json.JsonConverter(typeof(StringOrJsonStructureAsStringConverter))]
        public string HorasExtras { get; set; }

        [JsonPropertyName("inItinere")]
        public string InItinere { get; set; }
    }

    public class TotaisDTO
    {
        [JsonPropertyName("adicionalNoturno")]
        public string AdicionalNoturno { get; set; }

        [JsonPropertyName("adicionalNoturnoHorasExtras")]
        public string AdicionalNoturnoHorasExtras { get; set; }

        [JsonPropertyName("bancoHorasCredito")]
        public string BancoHorasCredito { get; set; }

        [JsonPropertyName("bancoHorasDebito")]
        public string BancoHorasDebito { get; set; }

        [JsonPropertyName("cPonte")]
        public string CPonte { get; set; }

        [JsonPropertyName("extras")]
        public string Extras { get; set; }

        [JsonPropertyName("faltas")]
        public string Faltas { get; set; }

        [JsonPropertyName("saldoAnterior")]
        public string SaldoAnterior { get; set; }

        [JsonPropertyName("saldoAtual")]
        public string SaldoAtual { get; set; }

        [JsonPropertyName("saldoPeriodo")]
        public string SaldoPeriodo { get; set; }

        [JsonPropertyName("trabalhadas")]
        public string Trabalhadas { get; set; }
    }
} 