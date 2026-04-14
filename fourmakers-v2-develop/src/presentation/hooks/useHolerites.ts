import { useState, useEffect, useMemo } from 'react';
import { container } from '@core/di/container';
import { GetHoleritesUseCase } from '@domain/usecases/GetHoleritesUseCase';
import type { Holerite, ListarHoleritesColaboradorPorAnoParams, AssinarHoleritePorLoteIdParams } from '@domain/entities/Holerite';
import { useAppSelector } from '@app/store/hooks';

export const useHolerites = (ano?: number) => {
  const { token, user } = useAppSelector((state) => state.auth);
  const [holerites, setHolerites] = useState<Holerite[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const paramsKey = useMemo(() => {
    if (!ano || !user?.cpf) return null;
    return JSON.stringify({
      codigoInternoColaborador: user.cpf,
      ano: ano,
    });
  }, [ano, user?.cpf]);

  useEffect(() => {
    const loadHolerites = async () => {
      if (!token || !user?.cpf || !ano) {
        setHolerites([]);
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const useCase = container.resolve(GetHoleritesUseCase);
        const params: ListarHoleritesColaboradorPorAnoParams = {
          codigoInternoColaborador: user.cpf,
          ano: ano,
        };
        const response = await useCase.executeListarHoleritesColaboradorPorAno(token, params);

        if (response.sucesso && response.retorno) {
          setHolerites(response.retorno);
        } else {
          setError(response.mensagem || 'Erro ao carregar holerites');
          setHolerites([]);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar holerites');
        setHolerites([]);
      } finally {
        setLoading(false);
      }
    };

    loadHolerites();
  }, [token, paramsKey]);

  const assinarHolerite = async (itemLoteId: string): Promise<boolean> => {
    if (!token) {
      setError('Token não encontrado');
      return false;
    }

    try {
      const useCase = container.resolve(GetHoleritesUseCase);
      const params: AssinarHoleritePorLoteIdParams = {
        itemLoteId: itemLoteId,
      };
      const response = await useCase.executeAssinarHoleritePorLoteId(token, params);

      if (response.sucesso) {
        // Atualizar o holerite na lista para marcar como assinado
        setHolerites((prev) =>
          prev.map((h) =>
            h.tbItemLoteId === itemLoteId
              ? { ...h, assinado: true, assinadoEm: new Date().toISOString() }
              : h
          )
        );
        return true;
      } else {
        setError(response.mensagem || 'Erro ao assinar holerite');
        return false;
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao assinar holerite');
      return false;
    }
  };

  return { holerites, loading, error, assinarHolerite };
};

