import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoFeedLabel } from '@domain/entities/comunicacao';
import type { ComunicacaoLabelApi } from '@data/api/ComunicacaoLabelApi';
import { ListarComunicacaoFeedUseCase } from '@domain/usecases/ListarComunicacaoFeedUseCase';

/** Lidos e pendentes por pasta, calculados a partir de publicacoes (a API não retorna por label). */
export type FolderStatsFromApi = Record<string, { lidos: number; pendentes: number }>;

/**
 * Calcula lidos/pendentes por label a partir do array de publicações.
 * A API retorna totais globais mesmo ao filtrar por label; usamos publicacoes[].labels e interação para obter por pasta.
 */
function computeFolderStatsFromPublicacoes(
  labels: ComunicacaoFeedLabel[],
  publicacoes: { labels?: string[]; requiresAcknowledgment?: boolean; acknowledgedAt?: string }[],
): FolderStatsFromApi {
  const stats: FolderStatsFromApi = {};
  for (const label of labels) {
    stats[label.nome] = { lidos: 0, pendentes: 0 };
  }
  for (const pub of publicacoes) {
    const labelNames = pub.labels ?? [];
    const isRead = !pub.requiresAcknowledgment || !!(pub.acknowledgedAt != null && pub.acknowledgedAt !== '');
    const isPending = !!pub.requiresAcknowledgment && !(pub.acknowledgedAt != null && pub.acknowledgedAt !== '');
    for (const nome of labelNames) {
      if (stats[nome]) {
        if (isRead) stats[nome].lidos += 1;
        if (isPending) stats[nome].pendentes += 1;
      }
    }
  }
  return stats;
}

export interface UseComunicacaoBibliotecaLabelsOptions {
  /** Quando false, não dispara o fetch inicial (ex.: só carregar ao abrir a aba Biblioteca). Default true. */
  enabled?: boolean;
}

/**
 * Busca a lista de labels (pastas de destino da biblioteca) via
 * POST api/Marketing/Comunicacao/Publicacao/ObterListaPublicacaoGeral com tipo: 'documento'.
 * Usado na tab Biblioteca e no modal Criar Novo Comunicado (Pastas de Destino).
 */
export function useComunicacaoBibliotecaLabels(options: UseComunicacaoBibliotecaLabelsOptions = {}) {
  const { enabled = true } = options;
  const token = useAppSelector((state) => state.auth.token);
  const listarComunicacaoFeedUseCase = container.resolve(ListarComunicacaoFeedUseCase);
  const comunicacaoLabelApi = container.resolve<ComunicacaoLabelApi>(DiTokens.comunicacaoLabelApi);

  const [labels, setLabels] = useState<ComunicacaoFeedLabel[]>([]);
  const [totals, setTotals] = useState<{
    quantidadeTotal: number;
    quantidadeLidosAceitos: number;
    quantidadePendentesLeitura: number;
  }>({
    quantidadeTotal: 0,
    quantidadeLidosAceitos: 0,
    quantidadePendentesLeitura: 0,
  });
  const [folderStatsFromApi, setFolderStatsFromApi] = useState<FolderStatsFromApi>({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadLabels = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const result = await listarComunicacaoFeedUseCase.execute(token, {
        somenteLeituraObrigatoria: false,
        tipo: 'documento',
        labels: [],
        comunidadeId: '',
      });
      const labelList = result.labels ?? [];
      setLabels(labelList);
      setTotals({
        quantidadeTotal: result.quantidadeTotal ?? 0,
        quantidadeLidosAceitos: result.quantidadeLidosAceitos ?? 0,
        quantidadePendentesLeitura: result.quantidadePendentesLeitura ?? 0,
      });
      setFolderStatsFromApi(
        computeFolderStatsFromPublicacoes(labelList, result.publicacoes ?? []),
      );
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Falha ao carregar pastas.');
      setLabels([]);
      setTotals({
        quantidadeTotal: 0,
        quantidadeLidosAceitos: 0,
        quantidadePendentesLeitura: 0,
      });
      setFolderStatsFromApi({});
    } finally {
      setLoading(false);
    }
  }, [token, listarComunicacaoFeedUseCase]);

  useEffect(() => {
    if (enabled && token) void loadLabels();
  }, [enabled, token, loadLabels]);

  const updateLabel = useCallback(
    async (labelId: string, nome: string): Promise<void> => {
      if (!token) return;
      await comunicacaoLabelApi.atualizar(token, labelId, { nome });
      await loadLabels();
    },
    [token, comunicacaoLabelApi, loadLabels],
  );

  const deleteLabel = useCallback(
    async (labelId: string): Promise<void> => {
      if (!token) return;
      await comunicacaoLabelApi.excluir(token, labelId);
      await loadLabels();
    },
    [token, comunicacaoLabelApi, loadLabels],
  );

  return {
    labels,
    totals,
    folderStatsFromApi,
    loading,
    error,
    refetch: loadLabels,
    updateLabel,
    deleteLabel,
  };
}
