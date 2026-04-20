import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import { DiTokens } from '@core/di/tokens';
import type { ComunicacaoComunidadeDetalheRetorno } from '@domain/entities/comunicacao';
import type { ComunicacaoComunidadeApi } from '@data/api/ComunicacaoComunidadeApi';

export function useComunicacaoComunidadeDetalhe(comunidadeId: string | null | undefined) {
  const token = useAppSelector((state) => state.auth.token);
  const [detalhe, setDetalhe] = useState<ComunicacaoComunidadeDetalheRetorno | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadDetalhe = useCallback(async () => {
    if (!token || !comunidadeId) {
      setDetalhe(null);
      return;
    }
    setLoading(true);
    setError(null);
    try {
      const api = container.resolve<ComunicacaoComunidadeApi>(DiTokens.comunicacaoComunidadeApi);
      const response = await api.getComunidadeById(token, comunidadeId);
      if (response.sucesso && response.retorno) {
        setDetalhe(response.retorno);
      } else {
        setDetalhe(null);
        setError(response.mensagem ?? 'Falha ao carregar detalhes da comunidade.');
      }
    } catch (e) {
      setDetalhe(null);
      setError(e instanceof Error ? e.message : 'Erro ao carregar detalhes da comunidade.');
    } finally {
      setLoading(false);
    }
  }, [token, comunidadeId]);

  useEffect(() => {
    if (comunidadeId && token) {
      loadDetalhe();
    } else {
      setDetalhe(null);
      setError(null);
    }
  }, [comunidadeId, token, loadDetalhe]);

  return { detalhe, loading, error, loadDetalhe };
}
