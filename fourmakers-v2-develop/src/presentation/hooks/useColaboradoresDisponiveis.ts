import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type { ColaboradorDisponivel } from '@domain/entities/comunicacao';
import { ListarColaboradoresDisponiveisUseCase } from '@domain/usecases/ListarColaboradoresDisponiveisUseCase';

const MIN_FILTRO_LENGTH = 2;

/**
 * Busca colaboradores disponíveis para o grupo.
 * Só chama a API quando filtro tem ao menos MIN_FILTRO_LENGTH caracteres (autocomplete leve).
 */
export function useColaboradoresDisponiveis(
  grupoId?: string,
  filtro?: string,
  enabled = true,
) {
  const token = useAppSelector((state) => state.auth.token);
  const useCase = container.resolve(ListarColaboradoresDisponiveisUseCase);

  const [colaboradores, setColaboradores] = useState<ColaboradorDisponivel[]>(
    [],
  );
  const [loading, setLoading] = useState(false);

  const shouldFetch =
    enabled &&
    !!token &&
    (filtro?.trim() ?? '').length >= MIN_FILTRO_LENGTH;

  const load = useCallback(async () => {
    if (!token || !shouldFetch) return;
    const f = (filtro?.trim() ?? '') || undefined;
    setLoading(true);
    try {
      const result = await useCase.execute(token, { grupoId, filtro: f });
      setColaboradores(result ?? []);
    } catch {
      setColaboradores([]);
    } finally {
      setLoading(false);
    }
  }, [token, grupoId, filtro, shouldFetch, useCase]);

  useEffect(() => {
    if (shouldFetch) {
      load();
    } else {
      setColaboradores([]);
      setLoading(false);
    }
  }, [shouldFetch, load]);

  return { colaboradores, loading, load };
}
