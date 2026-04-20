import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type { CommunityGroup } from '@domain/entities/comunicacao';
import { ListarComunicacaoComunidadesUseCase } from '@domain/usecases/ListarComunicacaoComunidadesUseCase';

export function useComunicacaoComunidades() {
  const token = useAppSelector((state) => state.auth.token);
  const listarComunidadesUseCase = container.resolve(
    ListarComunicacaoComunidadesUseCase,
  );

  const [comunidades, setComunidades] = useState<CommunityGroup[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadComunidades = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const result = await listarComunidadesUseCase.execute(token);
      setComunidades(result ?? []);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar comunidades.');
      setComunidades([]);
    } finally {
      setLoading(false);
    }
  }, [token, listarComunidadesUseCase]);

  useEffect(() => {
    if (token) {
      loadComunidades();
    }
  }, [token, loadComunidades]);

  return { comunidades, loading, error, loadComunidades };
}
