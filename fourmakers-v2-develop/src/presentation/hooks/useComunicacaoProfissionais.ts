import { useState, useCallback, useEffect } from 'react';
import { container } from 'tsyringe';
import { useAppSelector } from '@app/store/hooks';
import type { Professional } from '@domain/entities/comunicacao';
import { ListarComunicacaoProfissionaisUseCase } from '@domain/usecases/ListarComunicacaoProfissionaisUseCase';

export function useComunicacaoProfissionais() {
  const token = useAppSelector((state) => state.auth.token);
  const listarProfissionaisUseCase = container.resolve(
    ListarComunicacaoProfissionaisUseCase,
  );

  const [profissionais, setProfissionais] = useState<Professional[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProfissionais = useCallback(async () => {
    if (!token) return;
    setLoading(true);
    setError(null);
    try {
      const result = await listarProfissionaisUseCase.execute(token);
      setProfissionais(result ?? []);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar profissionais.');
      setProfissionais([]);
    } finally {
      setLoading(false);
    }
  }, [token, listarProfissionaisUseCase]);

  useEffect(() => {
    if (token) {
      loadProfissionais();
    }
  }, [token, loadProfissionais]);

  return { profissionais, loading, error, loadProfissionais };
}
