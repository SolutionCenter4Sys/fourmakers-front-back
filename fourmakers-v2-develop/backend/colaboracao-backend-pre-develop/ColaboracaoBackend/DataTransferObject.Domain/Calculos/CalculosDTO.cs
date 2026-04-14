using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Calculos
{
    public class CalcularSalarioLiquidoInputDTO
    {
        [JsonPropertyName("salarioBruto")]
        public decimal SalarioBruto { get; set; }

        [JsonPropertyName("numeroDependentes")]
        public int NumeroDependentes { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }
    }

    public class CalcularSalarioLiquidoOutputDTO
    {
        [JsonPropertyName("salarioBruto")]
        public decimal SalarioBruto { get; set; }

        [JsonPropertyName("numeroDependentes")]
        public int NumeroDependentes { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("descontoInss")]
        public decimal DescontoInss { get; set; }

        [JsonPropertyName("baseCalculoIrrf")]
        public decimal BaseCalculoIrrf { get; set; }

        [JsonPropertyName("descontoIrrf")]
        public decimal DescontoIrrf { get; set; }

        [JsonPropertyName("salarioLiquido")]
        public decimal SalarioLiquido { get; set; }

        [JsonPropertyName("totalDescontos")]
        public decimal TotalDescontos { get; set; }

        [JsonPropertyName("parametrosInss")]
        public ParametrosInssDTO ParametrosInss { get; set; }

        [JsonPropertyName("parametrosIrrf")]
        public ParametrosIrrfDTO ParametrosIrrf { get; set; }
    }

    public class ParametrosInssDTO
    {
        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("faixas")]
        public List<FaixaInssDTO> Faixas { get; set; }

        [JsonPropertyName("tetoInss")]
        public decimal TetoInss { get; set; }
    }

    public class FaixaInssDTO
    {
        [JsonPropertyName("faixa")]
        public int Faixa { get; set; }

        [JsonPropertyName("valorMinimo")]
        public decimal ValorMinimo { get; set; }

        [JsonPropertyName("valorMaximo")]
        public decimal ValorMaximo { get; set; }

        [JsonPropertyName("aliquota")]
        public decimal Aliquota { get; set; }
    }

    public class ParametrosIrrfDTO
    {
        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("faixas")]
        public List<FaixaIrrfDTO> Faixas { get; set; }

        [JsonPropertyName("deducaoPorDependente")]
        public decimal DeducaoPorDependente { get; set; }
    }

    public class FaixaIrrfDTO
    {
        [JsonPropertyName("faixa")]
        public int Faixa { get; set; }

        [JsonPropertyName("valorMinimo")]
        public decimal ValorMinimo { get; set; }

        [JsonPropertyName("valorMaximo")]
        public decimal ValorMaximo { get; set; }

        [JsonPropertyName("aliquota")]
        public decimal Aliquota { get; set; }

        [JsonPropertyName("deducao")]
        public decimal Deducao { get; set; }
    }

    public class SimularRemuneracaoTotalInputDTO
    {
        [JsonPropertyName("idVaga")]
        public Guid IdVaga { get; set; }

        [JsonPropertyName("liquidoPretendido")]
        public decimal LiquidoPretendido { get; set; }

        [JsonPropertyName("quantidadeDependentes")]
        public int QuantidadeDependentes { get; set; }

        [JsonPropertyName("alimentacao")]
        public decimal Alimentacao { get; set; }

        [JsonPropertyName("mobilidade")]
        public decimal Mobilidade { get; set; }

        [JsonPropertyName("educacao")]
        public decimal Educacao { get; set; }

        [JsonPropertyName("km")]
        public decimal Km { get; set; }

        /// <summary>Cargo de admissão (tb_admissao_cargo.id) enviado pelo front para carregar faixas em tb_admissao_remuneracao_clt.</summary>
        [JsonPropertyName("admissaoCargoId")]
        public Guid? AdmissaoCargoId { get; set; }

        /// <summary>Nível da vaga (1–4, tb_nivel_vaga.codigo) enviado pelo front para escolher faixa1_final..faixa4_final.</summary>
        [JsonPropertyName("nivelVagaCod")]
        public int? NivelVagaCod { get; set; }

        /// <summary>Custo total da vaga (ex.: custo hora × 168) informado pelo front. Quando preenchido, não consulta a vaga só para obter o custo.</summary>
        [JsonPropertyName("custoVagaProposto")]
        public decimal? CustoVagaProposto { get; set; }
    }

    public class SimularRemuneracaoTotalOutputDTO
    {
        [JsonPropertyName("primeiraOpcao")]
        public RemuneracaoDTO PrimeiraOpcao { get; set; }

        [JsonPropertyName("segundaOpcao")]
        public RemuneracaoDTO SegundaOpcao { get; set; }

        //[JsonPropertyName("emConformidadeComAPolitica")]
        //public bool EmConformidadeComAPolitica { get; set; }

        [JsonPropertyName("terceiraOpcao")]
        public RemuneracaoDTO TerceiraOpcao { get; set; }

        [JsonPropertyName("propostaIdeal")]
        public RemuneracaoDTO PropostaIdeal { get; set; }
    }

    /// <summary>
    /// Resultado do cálculo de verbas anuais (13º, FGTS, férias) para reutilização em proposta e pretendida.
    /// </summary>
    public class RemuneracaoVerbasAnuaisResult
    {
        public decimal DecimoTerceiroMensal { get; set; }
        public decimal FgtsMensal { get; set; }
        public decimal FeriasMensal { get; set; }
        public decimal FeriasLiquido { get; set; }
        public decimal DecimoTerceiroAnual { get; set; }
        public decimal FgtsAnual { get; set; }
    }

    public class ValidacaoPoliticaDTO
    {
        [JsonPropertyName("dentroDaPolitica")]
        public bool DentroDaPolitica { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }
    }

    public class RemuneracaoDTO
    {
        [JsonPropertyName("clt")]
        public CalcularSalarioLiquidoOutputDTO CLT { get; set; }

        [JsonPropertyName("validacaoCLT")]
        public ValidacaoPoliticaDTO ValidacaoCLT { get; set; }

        [JsonPropertyName("valeRefeicao")]
        public decimal ValeRefeicao { get; set; }

        [JsonPropertyName("validacaoValeRefeicao")]
        public ValidacaoPoliticaDTO ValidacaoValeRefeicao { get; set; }

        [JsonPropertyName("valeAlimentacao")]
        public decimal ValeAlimentacao { get; set; }

        [JsonPropertyName("validacaoValeAlimentacao")]
        public ValidacaoPoliticaDTO ValidacaoValeAlimentacao { get; set; }

        [JsonPropertyName("auxilioEducacao")]
        public decimal AuxilioEducacao { get; set; }

        [JsonPropertyName("validacaoAuxilioEducacao")]
        public ValidacaoPoliticaDTO ValidacaoAuxilioEducacao { get; set; }

        [JsonPropertyName("mobilidade")]
        public decimal Mobilidade { get; set; }

        [JsonPropertyName("validacaoMobilidade")]
        public ValidacaoPoliticaDTO ValidacaoMobilidade { get; set; }

        [JsonPropertyName("ajudaDeCusto")]
        public decimal AjudaDeCusto { get; set; }

        [JsonPropertyName("validacaoAjudaDeCusto")]
        public ValidacaoPoliticaDTO ValidacaoAjudaDeCusto { get; set; }

        [JsonPropertyName("remuneracaoTotalLiquidaMensal")]
        public decimal RemuneracaoTotalLiquidaMensal { get; set; }

        [JsonPropertyName("remuneracaoTotalLiquidaMensalComVerbasAnuais")]
        public decimal RemuneracaoTotalLiquidaMensalComVerbasAnuais { get; set; }

        [JsonPropertyName("previsaoAnual")]
        public decimal PrevisaoAnual { get; set; }

        [JsonPropertyName("custoTotalEmpresa")]
        public decimal CustoTotalEmpresa { get; set; }

        [JsonPropertyName("brutoComposto")]
        public decimal BrutoComposto { get; set; }

        [JsonPropertyName("custoVaga")]
        public decimal CustoVaga { get; set; }
    }
}