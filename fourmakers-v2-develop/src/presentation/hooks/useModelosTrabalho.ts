import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '@app/store/hooks';
import { fetchModelosTrabalho } from '@app/store/slices/modeloTrabalhoSlice';
import type { ModeloTrabalhoItem } from '@app/store/slices/modeloTrabalhoSlice';

export function useModelosTrabalho(effectiveToken: string | null) {
  const dispatch = useAppDispatch();
  const list = useAppSelector((state) => state.modeloTrabalho.list);
  const status = useAppSelector((state) => state.modeloTrabalho.status);

  useEffect(() => {
    if (!effectiveToken || list.length > 0 || status === 'loading') return;
    dispatch(fetchModelosTrabalho(effectiveToken));
  }, [effectiveToken, list.length, status, dispatch]);

  return {
    list: list as ModeloTrabalhoItem[],
    loading: status === 'loading',
    /** Refetch da API; com force=true ignora cache e sempre chama ListarModelosTrabalho. */
    refetch: (force?: boolean) =>
      effectiveToken && dispatch(fetchModelosTrabalho(force ? { token: effectiveToken, force: true } : effectiveToken)),
  };
}

/** Opções de dias presenciais para modelo híbrido (codigo === 2): label "X dia(s) presencial(ais)", valor enviado é X */
export const DIAS_PRESENCIAIS_OPCOES = [1, 2, 3, 4].map((n) => ({
  value: n === 1 ? '1 dia presencial' : `${n} dias presenciais`,
  label: n === 1 ? '1 dia presencial' : `${n} dias presenciais`,
}));
