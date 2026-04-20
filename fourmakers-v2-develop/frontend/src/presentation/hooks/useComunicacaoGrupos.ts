import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type { ComunicacaoGrupo } from '@domain/entities/comunicacao';
import { ListarComunicacaoGruposUseCase } from '@domain/usecases/ListarComunicacaoGruposUseCase';

export function useComunicacaoGrupos() {
  const token = useAppSelector((state) => state.auth.token);
  const listarGruposUseCase = container.resolve(ListarComunicacaoGruposUseCase);

  const [grupos, setGrupos] = useState<ComunicacaoGrupo[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadGrupos = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const result = await listarGruposUseCase.execute(token);
      setGrupos(result ?? []);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar grupos.');
      setGrupos([]);
    } finally {
      setLoading(false);
    }
  }, [token, listarGruposUseCase]);

  useEffect(() => {
    if (token) {
      loadGrupos();
    }
  }, [token, loadGrupos]);

  return { grupos, loading, error, loadGrupos };
}
