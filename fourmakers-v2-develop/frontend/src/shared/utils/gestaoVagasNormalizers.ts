/**
 * Normalizers para a feature Gestão de Vagas / Candidatos.
 * Usados pela camada de apresentação; tipos de domínio em @domain/entities/GestaoVagasCandidatos.
 */

import type {
  CandidatoInscrito,
  CandidatoInscritoRaw,
  TotaisInscritos,
  ObterTotaisInscritosRetorno,
} from '@domain/entities/GestaoVagasCandidatos';

/** Normaliza um candidato bruto da API para o modelo de UI (Kanban). */
export function normalizeCandidatoInscrito(raw: CandidatoInscritoRaw): CandidatoInscrito {
  return {
    id: raw.idCandidatura,
    idCandidatura: raw.idCandidatura,
    codigo: raw.codigo,
    nome: raw.nome,
    descricaoStatus: raw.descricaoStatusCandidatura,
    statusCandidaturaId: raw.idStatusCandidatura != null ? String(raw.idStatusCandidatura) : undefined,
    dataCandidatura: raw.candidatura,
    modificadoEm: raw.ultimaAlteracao,
    criadoPor: raw.nomeCompletoDeQuemCadastrou ?? undefined,
    percentualMatch: raw.match,
    tempoDecorridoTexto: raw.slaDecorridoTotal ?? undefined,
    origem: (raw as Record<string, unknown>).origem as string | undefined,
    totalInscritoOutrasVagas: raw.totalInscritoOutrasVagas ?? 0,
    recrutadorResponsavel: raw.recrutadorResponsavel ?? null,
    qualificado: raw.qualificado ?? null,
    dataQualificacao: raw.dataQualificacao ?? null,
    nomeDeQuemQualificou: raw.nomeDeQuemQualificou ?? null,
    retornoMatch: raw.retornoMatch ?? null,
    organizacoes: raw.organizacoes ?? [],
    slaDecorridoDaEtapaAtual: raw.slaDecorridoDaEtapaAtual ?? null,
    exibirRemuneracao: raw.exibirRemuneracao === true,
  };
}

/** Normaliza totais da API (totalCandidatosInscritos por status) para TotaisInscritos. Status 9=Aprovado, 10=Reprovado, 12=Declinado. */
export function normalizeTotaisInscritos(retorno: ObterTotaisInscritosRetorno | null | undefined): TotaisInscritos {
  const obj = retorno?.totalCandidatosInscritos ?? {};
  const values = Object.values(obj);
  const totalInscritos = values.reduce((acc, n) => acc + (typeof n === 'number' ? n : 0), 0);
  return {
    totalInscritos,
    totalAprovados: typeof obj['9'] === 'number' ? obj['9'] : 0,
    totalReprovados: typeof obj['10'] === 'number' ? obj['10'] : 0,
    totalDeclinados: typeof obj['12'] === 'number' ? obj['12'] : 0,
  };
}
