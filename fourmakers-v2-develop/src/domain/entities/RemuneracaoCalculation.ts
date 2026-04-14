export type ValidationFieldName =
  | 'clt'
  | 'valeRefeicao'
  | 'valeAlimentacao'
  | 'auxilioEducacao'
  | 'mobilidade'
  | 'ajudaDeCusto';

export interface RemuneracaoValidation {
  dentroDaPolitica: boolean;
  mensagem: string;
}

export interface RemuneracaoCltDetails {
  salarioBruto?: number | null;
  numeroDependentes?: number | null;
  ano?: number | null;
  descontoInss?: number | null;
  baseCalculoIrrf?: number | null;
  descontoIrrf?: number | null;
  salarioLiquido?: number | null;
  totalDescontos?: number | null;
}

export interface RemuneracaoProposta {
  clt?: RemuneracaoCltDetails | null;
  validacaoCLT?: RemuneracaoValidation | null;
  valeRefeicao?: number | null;
  validacaoValeRefeicao?: RemuneracaoValidation | null;
  valeAlimentacao?: number | null;
  validacaoValeAlimentacao?: RemuneracaoValidation | null;
  auxilioEducacao?: number | null;
  validacaoAuxilioEducacao?: RemuneracaoValidation | null;
  mobilidade?: number | null;
  validacaoMobilidade?: RemuneracaoValidation | null;
  ajudaDeCusto?: number | null;
  validacaoAjudaDeCusto?: RemuneracaoValidation | null;
  remuneracaoTotalLiquidaMensal?: number | null;
  remuneracaoTotalLiquidaMensalComVerbasAnuais?: number | null;
  previsaoAnual?: number | null;
  custoTotalEmpresa?: number | null;
}

export interface RemuneracaoPretendida {
  clt?: RemuneracaoCltDetails | null;
  validacaoCLT?: RemuneracaoValidation | null;
  valeRefeicao?: number | null;
  validacaoValeRefeicao?: RemuneracaoValidation | null;
  valeAlimentacao?: number | null;
  validacaoValeAlimentacao?: RemuneracaoValidation | null;
  auxilioEducacao?: number | null;
  validacaoAuxilioEducacao?: RemuneracaoValidation | null;
  mobilidade?: number | null;
  validacaoMobilidade?: RemuneracaoValidation | null;
  ajudaDeCusto?: number | null;
  validacaoAjudaDeCusto?: RemuneracaoValidation | null;
  remuneracaoTotalLiquidaMensal?: number | null;
  remuneracaoTotalLiquidaMensalComVerbasAnuais?: number | null;
  previsaoAnual?: number | null;
  custoTotalEmpresa?: number | null;
}

export interface RemuneracaoCalculationPayload {
  idVaga: string;
  liquidoPretendido: number;
  quantidadeDependentes: number;
  alimentacao: number;
  mobilidade: number;
  educacao: number;
  ajudaDeCusto: number;
  km: number;
}

export interface RemuneracaoCalculationResponse {
  retorno: {
    remuneracaoProposta?: RemuneracaoProposta | null;
    remuneracaoPretendida?: RemuneracaoPretendida | null;
    /** Indica se a proposta está em conformidade com a política (API simulador). */
    emConformidadeComAPolitica?: boolean | null;
  };
  sucesso?: boolean;
  mensagem?: string | null;
  erros?: unknown | null;
}
