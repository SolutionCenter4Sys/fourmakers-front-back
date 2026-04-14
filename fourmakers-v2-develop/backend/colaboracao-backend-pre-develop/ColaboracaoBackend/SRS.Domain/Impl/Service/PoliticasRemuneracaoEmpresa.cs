using System;
using DataTransferObject.Domain.SRS.RemuneracaoClt;

namespace SRS.Domain.Impl.Service
{
    /// <summary>
    /// Classe que contém as políticas de remuneração da empresa
    /// </summary>
    public static class PoliticasRemuneracaoEmpresa
    {
        /// <summary>
        /// CLT bruto da <b>proposta</b>: 50% do valor de referência (faixa final conforme nível da vaga em tb_admissao_remuneracao_clt) quando existir; senão 50% do custo da vaga.
        /// </summary>
        public const decimal PercentualCltPropostaSobreBaseTabelaOuCustoVaga = 0.50m; // 50%

        /// <summary>
        /// Percentual mínimo que o salário CLT (bruto) deve representar do BrutoTotal
        /// (CLT + AjudaDeCusto + Mobilidade + Vale Alimentação + Vale Refeição).
        /// Usado na validação da remuneração <b>pretendida</b>.
        /// </summary>
        public const decimal PercentualMinimoCLTSobreBrutoTotal = 0.60m; // 60%

        /// <summary>
        /// Percentual máximo que a Ajuda de Custo pode representar do CLT
        /// </summary>
        public const decimal PercentualMaximoAjudaCustoSobreCLT = 0.20m; // 20%

        /// <summary>
        /// Valor limite para determinar o percentual máximo de Alimentação
        /// </summary>
        public const decimal LimiteRendimentoTotalParaAlimentacao = 10000.00m;

        /// <summary>
        /// Percentual máximo de Alimentação sobre CLT quando RendimentoTotal > 10000
        /// </summary>
        public const decimal PercentualMaximoAlimentacaoAltoRendimento = 0.19m; // 19%

        /// <summary>
        /// Percentual máximo de Alimentação sobre CLT quando RendimentoTotal <= 10000
        /// </summary>
        public const decimal PercentualMaximoAlimentacaoBaixoRendimento = 0.10m; // 10%

        /// <summary>
        /// Percentual máximo que o Auxílio Educação pode representar do CLT
        /// </summary>
        public const decimal PercentualMaximoAuxilioEducacaoSobreCLT = 0.25m; // 25%

        /// <summary>
        /// Percentual máximo que a Mobilidade pode representar do CLT Bruto
        /// </summary>
        public const decimal PercentualMaximoMobilidadeSobreCLT = 0.50m; // 50%

        public const decimal ValeRefeicao = 704m;
        public const decimal AssistenciaMedica = 520.28m;

        /// <summary>
        /// SalarioMinimoVigente
        /// </summary>
        public const decimal SalarioMinimoVigente = 2500m;

        /// <summary>
        /// BrutoTotal da proposta: soma de CLT (bruto) + AjudaDeCusto + Mobilidade + VA + VR + auxEducacao.
        /// </summary>
        public static decimal CalcularBrutoTotalRemuneracao(
            decimal cltBruto,
            decimal ajudaCusto,
            decimal mobilidade,
            decimal valeAlimentacao,
            decimal valeRefeicao,
            decimal auxEducacao,
            decimal assMedica)
        {
            return cltBruto + ajudaCusto + mobilidade + valeAlimentacao + valeRefeicao + auxEducacao + assMedica;
        }

        /// <summary>
        /// Seleciona <c>faixaN_final</c> em <paramref name="registro"/> conforme <paramref name="nivelVagaCod"/> (1–4).
        /// </summary>
        public static decimal? ObterValorBaseFaixaFinalPorNivel(RemuneracaoCltResult registro, int nivelVagaCod)
        {
            if (registro == null || nivelVagaCod < 1 || nivelVagaCod > 4)
                return null;

            decimal? faixa = nivelVagaCod switch
            {
                1 => registro.Faixa1Final,
                2 => registro.Faixa2Final,
                3 => registro.Faixa3Final,
                4 => registro.Faixa4Final,
                _ => null
            };

            if (!faixa.HasValue || faixa.Value <= 0m)
                return null;

            return faixa.Value;
        }

        /// <summary>
        /// Salário CLT (bruto) da proposta: 50% do valor base da tabela (faixa final do nível) quando informado; caso contrário 50% do custo da vaga.
        /// </summary>
        public static decimal CalcularCltBrutoPropostaInicial(decimal custoVaga, decimal? valorBaseRemuneracaoClt)
        {
            if (valorBaseRemuneracaoClt.HasValue && valorBaseRemuneracaoClt.Value > 0m)
                return valorBaseRemuneracaoClt.Value * PercentualCltPropostaSobreBaseTabelaOuCustoVaga;
            return custoVaga * PercentualCltPropostaSobreBaseTabelaOuCustoVaga;
        }

        /// <summary>
        /// Mínimo de CLT (bruto) para o CLT representar pelo menos 60% do BrutoTotal
        /// (validação da remuneração pretendida).
        /// </summary>
        public static (decimal, string) CalcularValorMinimoCLTBrutoPelaRegra60PorcentoBrutoTotalOu50PorCentoDoPiso(
            decimal ajudaCusto,
            decimal mobilidade,
            decimal valeAlimentacao,
            decimal valeRefeicao,
            decimal cltBruto)
        {
            decimal somaSemClt = ajudaCusto + mobilidade + valeAlimentacao + valeRefeicao;
            var resultadoPelos60PorCento = somaSemClt * (PercentualMinimoCLTSobreBrutoTotal / (1m - PercentualMinimoCLTSobreBrutoTotal));
            //var metadePiso = registro.Piso.Value * 0.5m;
            //if (metadePiso < resultadoPelos60PorCento)
            //    return (metadePiso, "(50% do piso)");
            //else

            if (resultadoPelos60PorCento < SalarioMinimoVigente)
                return (SalarioMinimoVigente, "Minimo Vigente");
            else
                return (resultadoPelos60PorCento, "(60% do BrutoTotal)");
        }

        /// <summary>
        /// Calcula o valor máximo que a Ajuda de Custo pode ter baseado no CLT
        /// </summary>
        /// <param name="valorCLT">Valor do CLT</param>
        /// <returns>Valor máximo da Ajuda de Custo</returns>
        public static decimal CalcularValorMaximoAjudaCusto(decimal valorCLT)
        {
            return valorCLT * PercentualMaximoAjudaCustoSobreCLT;
        }

        /// <summary>
        /// Calcula o valor máximo que a Alimentação pode ter baseado no CLT e RendimentoTotal
        /// </summary>
        /// <param name="valorCLT">Valor do CLT</param>
        /// <param name="brutoCompostoTotal">Rendimento total disponível</param>
        /// <returns>Valor máximo da Alimentação</returns>
        public static decimal CalcularValorMaximoAlimentacao(decimal valorCLT, decimal brutoCompostoTotal)
        {
            if (brutoCompostoTotal > LimiteRendimentoTotalParaAlimentacao)
                return brutoCompostoTotal * PercentualMaximoAlimentacaoBaixoRendimento;

            return valorCLT * PercentualMaximoAlimentacaoAltoRendimento;
        }

        /// <summary>
        /// Calcula o valor máximo que o Auxílio Educação pode ter baseado no CLT
        /// </summary>
        /// <param name="valorCLT">Valor do CLT</param>
        /// <returns>Valor máximo do Auxílio Educação</returns>
        public static decimal CalcularValorMaximoAuxilioEducacao(decimal valorCLT)
        {
            return valorCLT * PercentualMaximoAuxilioEducacaoSobreCLT;
        }

        /// <summary>
        /// Calcula o valor máximo que a Mobilidade pode ter baseado no CLT Bruto
        /// </summary>
        /// <param name="valorCLT">Valor do CLT Bruto</param>
        /// <returns>Valor máximo da Mobilidade</returns>
        public static decimal CalcularValorMaximoMobilidade(decimal valorCLT)
        {
            return valorCLT * PercentualMaximoMobilidadeSobreCLT;
        }
    }
}

