import { useCallback, useEffect, useRef } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  listarIniciativasPorPosicaoId,
  buscarIniciativaPorId,
  criarIniciativa,
  atualizarIniciativa,
  excluirIniciativa,
  listarStatusIniciativas,
  listarTemas,
  setIniciativaSelecionada,
  limparIniciativas,
  limparIniciativasPorPosicao,
  resetVcxIniciativas,
} from '@app/store/slices/vcxIniciativasSlice'
import type { IniciativaPayload } from '@domain/entities/VcxIniciativas'
import { toast } from 'sonner'
import { logUserAction } from '@shared/utils/firebaseAnalytics'

/**
 * Hook para gerenciar operações de Iniciativas VCX
 * 
 * @example
 * ```tsx
 * const { 
 *   iniciativas, 
 *   isLoading, 
 *   carregarIniciativas, 
 *   criarNovaIniciativa 
 * } = useVcxIniciativas()
 * 
 * useEffect(() => {
 *   if (posicaoId) {
 *     carregarIniciativas(posicaoId)
 *   }
 * }, [posicaoId])
 * ```
 */
export function useVcxIniciativas() {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const {
    iniciativas,
    iniciativasPorPosicao,
    iniciativaSelecionada,
    statusIniciativas,
    temas,
    status,
    error,
    statusListar,
    statusCriar,
    statusAtualizar,
    statusExcluir,
    statusDadosReferencia,
  } = useAppSelector((state) => state.vcxIniciativas)

  /**
   * Carrega as iniciativas de uma posição específica
   */
  const carregarIniciativas = useCallback(
    async (posicaoId: string) => {
      if (!token || !posicaoId) {
        return
      }

      try {
        await dispatch(
          listarIniciativasPorPosicaoId({ token, posicaoId }),
        ).unwrap()
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao carregar iniciativas'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token],
  )

  /**
   * Busca uma iniciativa específica por ID
   */
  const buscarIniciativa = useCallback(
    async (id: string) => {
      if (!token || !id) {
        return
      }

      try {
        const iniciativa = await dispatch(
          buscarIniciativaPorId({ token, id }),
        ).unwrap()
        return iniciativa
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao buscar iniciativa'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token],
  )

  /**
   * Cria uma nova iniciativa com tema
   */
  const criarNovaIniciativa = useCallback(
    async (payload: IniciativaPayload, temaDescricao: string) => {
      if (!token) {
        throw new Error('Token não disponível')
      }

      try {
        const novaIniciativa = await dispatch(
          criarIniciativa({ token, payload, temaDescricao }),
        ).unwrap()
        toast.success('Iniciativa criada com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'CriarIniciativa',
            {
              posicaoId: payload.organogramaPosicaoId,
              temStatus: !!payload.vcxStatusId,
              temTema: !!payload.vcxTemasId,
              temDescricao: !!payload.descricao,
              temaDescricao,
            },
            user,
          )
        }
        
        return novaIniciativa
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao criar iniciativa'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Atualiza uma iniciativa existente com tema
   */
  const atualizarIniciativaExistente = useCallback(
    async (payload: IniciativaPayload & { id: string }, temaDescricao: string) => {
      if (!token) {
        throw new Error('Token não disponível')
      }

      try {
        const iniciativaAtualizada = await dispatch(
          atualizarIniciativa({ token, payload, temaDescricao }),
        ).unwrap()
        toast.success('Iniciativa atualizada com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'AtualizarIniciativa',
            {
              iniciativaId: payload.id,
              posicaoId: payload.organogramaPosicaoId,
              temStatus: !!payload.vcxStatusId,
              temTema: !!payload.vcxTemasId,
              temDescricao: !!payload.descricao,
              temaDescricao,
            },
            user,
          )
        }
        
        return iniciativaAtualizada
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao atualizar iniciativa'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Exclui uma iniciativa
   */
  const excluirIniciativaExistente = useCallback(
    async (id: string) => {
      if (!token || !id) {
        return
      }

      try {
        await dispatch(excluirIniciativa({ token, id })).unwrap()
        toast.success('Iniciativa excluída com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'ExcluirIniciativa',
            {
              iniciativaId: id,
            },
            user,
          )
        }
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao excluir iniciativa'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Carrega os dados de referência (Status e Temas).
   * Usa allSettled para que falha em um (ex.: temas vazio/nulo) não impeça o outro de popular o estado.
   */
  const carregarDadosReferencia = useCallback(async () => {
    if (!token) return

    const [statusResult, temasResult] = await Promise.allSettled([
      dispatch(listarStatusIniciativas({ token })).unwrap(),
      dispatch(listarTemas({ token })).unwrap(),
    ])
    if (statusResult.status === 'rejected') {
      toast.error(
        statusResult.reason instanceof Error
          ? statusResult.reason.message
          : 'Erro ao carregar status de iniciativas',
      )
    }
    if (temasResult.status === 'rejected') {
      toast.error(
        temasResult.reason instanceof Error
          ? temasResult.reason.message
          : 'Erro ao carregar temas',
      )
    }
  }, [dispatch, token])


  /**
   * Filtra temas baseado em um termo de busca
   * @param termo - Termo para filtrar temas
   * @returns Lista de temas filtrados
   */
  const filtrarTemas = useCallback(
    (termo: string) => {
      if (!termo.trim()) {
        return temas
      }

      const termoLower = termo.toLowerCase()
      return temas.filter((tema) =>
        tema.descricao.toLowerCase().includes(termoLower),
      )
    },
    [temas],
  )

  /**
   * Seleciona uma iniciativa
   */
  const selecionarIniciativa = useCallback(
    (iniciativa: typeof iniciativaSelecionada) => {
      dispatch(setIniciativaSelecionada(iniciativa))
    },
    [dispatch],
  )

  /**
   * Limpa todas as iniciativas
   */
  const limparTodasIniciativas = useCallback(() => {
    dispatch(limparIniciativas())
  }, [dispatch])

  /**
   * Limpa as iniciativas de uma posição específica
   */
  const limparIniciativasDaPosicao = useCallback(
    (posicaoId: string) => {
      dispatch(limparIniciativasPorPosicao(posicaoId))
    },
    [dispatch],
  )

  /**
   * Reseta todo o estado do slice
   */
  const resetarEstado = useCallback(() => {
    dispatch(resetVcxIniciativas())
  }, [dispatch])

  // Carregar dados de referência uma vez por token (evita loop quando temas retorna vazio/null)
  const dadosCarregadosRef = useRef(false)
  useEffect(() => {
    if (!token) {
      dadosCarregadosRef.current = false
      return
    }
    if (dadosCarregadosRef.current) return
    dadosCarregadosRef.current = true
    carregarDadosReferencia()
  }, [token, carregarDadosReferencia])

  return {
    // Dados
    iniciativas,
    iniciativasPorPosicao,
    iniciativaSelecionada,
    statusIniciativas,
    temas,

    // Estados
    isLoading: status === 'loading',
    isLoadingListar: statusListar === 'loading',
    isLoadingCriar: statusCriar === 'loading',
    isLoadingAtualizar: statusAtualizar === 'loading',
    isLoadingExcluir: statusExcluir === 'loading',
    isLoadingDadosReferencia: statusDadosReferencia === 'loading',
    error,

    // Funções
    carregarIniciativas,
    buscarIniciativa,
    criarNovaIniciativa,
    atualizarIniciativaExistente,
    excluirIniciativaExistente,
    carregarDadosReferencia,
    filtrarTemas,
    selecionarIniciativa,
    limparTodasIniciativas,
    limparIniciativasDaPosicao,
    resetarEstado,
  }
}
