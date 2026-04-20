import { useCallback, useEffect, useRef } from 'react'
import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import {
  listarDoresPorPosicaoId,
  buscarDorPorId,
  criarDor,
  atualizarDor,
  excluirDor,
  listarImpactosDores,
  listarUrgenciasDores,
  setDorSelecionada,
  limparDores,
  limparDoresPorPosicao,
  resetVcxDores,
} from '@app/store/slices/vcxDoresSlice'
import type { DorPayload } from '@domain/entities/VcxDores'
import { toast } from 'sonner'
import { logUserAction } from '@shared/utils/firebaseAnalytics'

/**
 * Hook para gerenciar operações de Dores VCX
 * 
 * @example
 * ```tsx
 * const { 
 *   dores, 
 *   isLoading, 
 *   carregarDores, 
 *   criarNovaDor 
 * } = useVcxDores()
 * 
 * useEffect(() => {
 *   if (posicaoId) {
 *     carregarDores(posicaoId)
 *   }
 * }, [posicaoId])
 * ```
 */
export function useVcxDores() {
  const dispatch = useAppDispatch()
  const { token, user } = useAppSelector((state) => state.auth)
  const {
    dores,
    doresPorPosicao,
    dorSelecionada,
    impactos,
    urgencias,
    status,
    error,
    statusListar,
    statusCriar,
    statusAtualizar,
    statusExcluir,
    statusDadosReferencia,
  } = useAppSelector((state) => state.vcxDores)

  /**
   * Carrega as dores de uma posição específica
   */
  const carregarDores = useCallback(
    async (posicaoId: string) => {
      if (!token || !posicaoId) {
        return
      }

      try {
        await dispatch(
          listarDoresPorPosicaoId({ token, posicaoId }),
        ).unwrap()
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao carregar dores'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token],
  )

  /**
   * Busca uma dor específica por ID
   */
  const buscarDor = useCallback(
    async (id: string) => {
      if (!token || !id) {
        return
      }

      try {
        const dor = await dispatch(buscarDorPorId({ token, id })).unwrap()
        return dor
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao buscar dor'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token],
  )

  /**
   * Cria uma nova dor
   */
  const criarNovaDor = useCallback(
    async (payload: DorPayload) => {
      if (!token) {
        throw new Error('Token não disponível')
      }

      try {
        const novaDor = await dispatch(criarDor({ token, payload })).unwrap()
        toast.success('Dor criada com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'CriarDor',
            {
              posicaoId: payload.organogramaPosicaoId,
              temImpacto: !!payload.vcxImpactosId,
              temUrgencia: !!payload.vcxUrgenciasId,
              temDescricao: !!payload.descricao,
            },
            user,
          )
        }
        
        return novaDor
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao criar dor'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Atualiza uma dor existente
   */
  const atualizarDorExistente = useCallback(
    async (payload: DorPayload & { id: string }) => {
      if (!token) {
        throw new Error('Token não disponível')
      }

      try {
        const dorAtualizada = await dispatch(
          atualizarDor({ token, payload }),
        ).unwrap()
        toast.success('Dor atualizada com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'AtualizarDor',
            {
              dorId: payload.id,
              posicaoId: payload.organogramaPosicaoId,
              temImpacto: !!payload.vcxImpactosId,
              temUrgencia: !!payload.vcxUrgenciasId,
              temDescricao: !!payload.descricao,
            },
            user,
          )
        }
        
        return dorAtualizada
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao atualizar dor'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Exclui uma dor
   */
  const excluirDorExistente = useCallback(
    async (id: string) => {
      if (!token || !id) {
        return
      }

      try {
        await dispatch(excluirDor({ token, id })).unwrap()
        toast.success('Dor excluída com sucesso')
        
        // Rastreamento Firebase Analytics
        if (user) {
          logUserAction(
            'MapaRelacionamento',
            'ExcluirDor',
            {
              dorId: id,
            },
            user,
          )
        }
      } catch (err) {
        const message =
          err instanceof Error ? err.message : 'Erro ao excluir dor'
        toast.error(message)
        throw err
      }
    },
    [dispatch, token, user],
  )

  /**
   * Carrega os dados de referência (Impactos e Urgências).
   * Usa allSettled para que falha em um não impeça o outro de popular o estado (evita loading/disabled quando um retorna [] ou falha).
   */
  const carregarDadosReferencia = useCallback(async () => {
    if (!token) {
      return
    }

    const [impactosResult, urgenciasResult] = await Promise.allSettled([
      dispatch(listarImpactosDores({ token })).unwrap(),
      dispatch(listarUrgenciasDores({ token })).unwrap(),
    ])
    if (impactosResult.status === 'rejected') {
      toast.error(
        impactosResult.reason instanceof Error
          ? impactosResult.reason.message
          : 'Erro ao carregar impactos',
      )
    }
    if (urgenciasResult.status === 'rejected') {
      toast.error(
        urgenciasResult.reason instanceof Error
          ? urgenciasResult.reason.message
          : 'Erro ao carregar urgências',
      )
    }
  }, [dispatch, token])

  /**
   * Seleciona uma dor
   */
  const selecionarDor = useCallback(
    (dor: typeof dorSelecionada) => {
      dispatch(setDorSelecionada(dor))
    },
    [dispatch],
  )

  /**
   * Limpa todas as dores
   */
  const limparTodasDores = useCallback(() => {
    dispatch(limparDores())
  }, [dispatch])

  /**
   * Limpa as dores de uma posição específica
   */
  const limparDoresDaPosicao = useCallback(
    (posicaoId: string) => {
      dispatch(limparDoresPorPosicao(posicaoId))
    },
    [dispatch],
  )

  /**
   * Reseta todo o estado do slice
   */
  const resetarEstado = useCallback(() => {
    dispatch(resetVcxDores())
  }, [dispatch])

  // Carregar dados de referência uma vez por token (evita loop quando um endpoint retorna vazio).
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
    dores,
    doresPorPosicao,
    dorSelecionada,
    impactos,
    urgencias,

    // Estados
    isLoading: status === 'loading',
    isLoadingListar: statusListar === 'loading',
    isLoadingCriar: statusCriar === 'loading',
    isLoadingAtualizar: statusAtualizar === 'loading',
    isLoadingExcluir: statusExcluir === 'loading',
    isLoadingDadosReferencia: statusDadosReferencia === 'loading',
    error,

    // Funções
    carregarDores,
    buscarDor,
    criarNovaDor,
    atualizarDorExistente,
    excluirDorExistente,
    carregarDadosReferencia,
    selecionarDor,
    limparTodasDores,
    limparDoresDaPosicao,
    resetarEstado,
  }
}
