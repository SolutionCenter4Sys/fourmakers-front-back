import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoLabelApi } from '@data/api/ComunicacaoLabelApi';
import type { ComunicacaoLabelItem } from '@data/api/ComunicacaoLabelApi';

export interface UseComunicacaoLabelsOptions {
  /** Quando true, dispara o fetch ao montar. Default true. */
  enabled?: boolean;
}

/**
 * Busca a lista de labels (tags) via GET api/Marketing/Comunicacao/Label.
 * Usado no filtro do feed e no modal Criar Novo Comunicado (Tags).
 */
export function useComunicacaoLabels(options: UseComunicacaoLabelsOptions = {}) {
  const { enabled = true } = options;
  const token = useAppSelector((state) => state.auth.token);
  const api = container.resolve<ComunicacaoLabelApi>(DiTokens.comunicacaoLabelApi);

  const [labels, setLabels] = useState<ComunicacaoLabelItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadLabels = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const list = await api.listar(token);
      setLabels(list ?? []);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar tags.');
      setLabels([]);
    } finally {
      setLoading(false);
    }
  }, [token, api]);

  useEffect(() => {
    if (enabled && token) void loadLabels();
  }, [enabled, token, loadLabels]);

  return { labels, loading, error, loadLabels };
}
