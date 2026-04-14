import type { ModeloTrabalho } from '@domain/entities/GestaoVagasCandidatos';

/**
 * Mock só para testes locais isolados. Na candidatura pública (/public/vaga) sempre use a API
 * (ids válidos por org); manter false evita "modelo de trabalho inválido" no CandidatarSe.
 */
export const USE_MOCK_MODELOS_TRABALHO = false;

/** Retorno mockado conforme HML (ListarModelosTrabalho). */
export const MOCK_MODELOS_TRABALHO: ModeloTrabalho[] = [
  { id: 'db53c699-ac2d-11ef-9eb3-0e1e12942759', descricao: '100% Presencial', codigo: 1 },
  { id: 'db822243-ac2d-11ef-9eb3-0e1e12942759', descricao: '100% Remoto', codigo: 3 },
  { id: 'dbb5339d-ac2d-11ef-9eb3-0e1e12942759', descricao: 'Híbrido', codigo: 2 },
];
