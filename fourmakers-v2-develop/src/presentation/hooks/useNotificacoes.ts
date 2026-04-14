import { useEffect, useState, useCallback } from 'react';
import { useAppSelector } from '@app/store/hooks';
import { container } from '@core/di/container';
import { ContarNotificacoesNaoLidasUseCase } from '@domain/usecases/ContarNotificacoesNaoLidasUseCase';

export const useNotificacoes = () => {
  const { token } = useAppSelector((state) => state.auth);
  const [contador, setContador] = useState(0);
  const [isLoading, setIsLoading] = useState(false);

  const atualizarContador = useCallback(async () => {
    if (!token) {
      setContador(0);
      return;
    }

    setIsLoading(true);
    try {
      const useCase = container.resolve(ContarNotificacoesNaoLidasUseCase);
      const count = await useCase.execute(token);
      setContador(count);
    } catch (error) {
      console.error('Erro ao atualizar contador de notificações:', error);
      // Não exibir erro para o usuário, apenas logar
    } finally {
      setIsLoading(false);
    }
  }, [token]);

  useEffect(() => {
    if (!token) return;

    // Carregar imediatamente
    atualizarContador();

    // Configurar polling a cada 1 minuto (60000ms)
    const intervalId = setInterval(() => {
      atualizarContador();
    }, 60000);

    return () => {
      clearInterval(intervalId);
    };
  }, [token, atualizarContador]);

  return {
    contador,
    isLoading,
    atualizarContador,
  };
};

