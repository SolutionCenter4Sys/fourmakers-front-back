import { useState, useEffect, useCallback, useRef } from 'react'
import { container } from '@core/di/container'
import { ListarGestoresUseCase } from '@domain/usecases/ListarGestoresUseCase'
import type { GestorItem } from '@domain/repositories/PerfilAtuacaoRepository'

const LIMITE_PAGINA = 50

export interface GestorExterno {
  codGestorExterno: string
  codigoInternoColaborador: string
  nome: string
  email: string
}

interface UseGestoresExternosParams {
  token: string | null
  codigoCliente: string | null
  busca?: string
  enabled?: boolean
  refreshTrigger?: number // Trigger para forçar refresh
  /** Quando true, usa paginação para scroll infinito (limite 50 por página) */
  paginado?: boolean
}

/**
 * Adapter para converter GestorItem (formato do repository) para GestorExterno (formato esperado pelo hook)
 */
function adapterGestorItemToGestorExterno(gestorItem: GestorItem): GestorExterno {
  return {
    codGestorExterno: gestorItem.codGestorExterno || gestorItem.id,
    codigoInternoColaborador: gestorItem.id,
    nome: gestorItem.descricao,
    email: gestorItem.email || '',
  }
}

export function useGestoresExternos({
  token,
  codigoCliente,
  busca = '',
  enabled = true,
  refreshTrigger = 0,
  paginado = false,
}: UseGestoresExternosParams) {
  const [gestores, setGestores] = useState<GestorExterno[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [hasMore, setHasMore] = useState(true)
  const [loadingMore, setLoadingMore] = useState(false)
  const cursorRef = useRef(0)
  const loadingMoreRef = useRef(false)
  /** Fallback: quando a API ignora paginação e retorna tudo, armazenamos para paginar no cliente */
  const bufferRef = useRef<GestorExterno[]>([])

  const [debouncedBusca, setDebouncedBusca] = useState(busca)

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedBusca(busca)
    }, 300)

    return () => clearTimeout(timer)
  }, [busca])

  const loadGestores = useCallback(
    async (cursor: number, append: boolean) => {
      if (!enabled || !token) return
      const limite = paginado ? LIMITE_PAGINA : 5000
      if (append) {
        loadingMoreRef.current = true
        setLoadingMore(true)
      } else {
        setLoading(true)
        bufferRef.current = []
      }
      setError(null)

      try {
        const useCase = container.resolve(ListarGestoresUseCase)
        const gestoresItems = await useCase.execute(
          token,
          codigoCliente ?? null,
          debouncedBusca.trim(),
          cursor,
          limite,
        )
        const todosMapeados = gestoresItems.map(adapterGestorItemToGestorExterno)

        if (paginado && todosMapeados.length > limite) {
          // API ignorou paginação e retornou tudo — paginar no cliente
          bufferRef.current = todosMapeados
          const exibir = append
            ? bufferRef.current.slice(0, cursorRef.current + limite)
            : todosMapeados.slice(0, limite)
          setGestores(exibir)
          setHasMore(cursorRef.current + limite < todosMapeados.length)
          cursorRef.current = Math.min(cursorRef.current + limite, todosMapeados.length)
        } else {
          if (append) {
            setGestores((prev) => {
              const ids = new Set(prev.map((g) => g.codGestorExterno))
              const novos = todosMapeados.filter((g) => !ids.has(g.codGestorExterno))
              return [...prev, ...novos]
            })
          } else {
            setGestores(todosMapeados)
          }
          setHasMore(paginado && todosMapeados.length === limite)
          cursorRef.current = cursor + todosMapeados.length
        }
      } catch (err) {
        console.error('Erro ao buscar gestores externos:', err)
        setError(err instanceof Error ? err.message : 'Erro ao buscar gestores externos')
        if (!append) setGestores([])
      } finally {
        setLoading(false)
        setLoadingMore(false)
        loadingMoreRef.current = false
      }
    },
    [token, codigoCliente, debouncedBusca, enabled, paginado],
  )

  const loadMore = useCallback(() => {
    if (!hasMore || loadingMoreRef.current) return
    if (bufferRef.current.length > 0) {
      // Modo client-side: próximo lote do buffer
      const nextEnd = Math.min(cursorRef.current + LIMITE_PAGINA, bufferRef.current.length)
      const proximo = bufferRef.current.slice(cursorRef.current, nextEnd)
      setGestores((prev) => [...prev, ...proximo])
      cursorRef.current = nextEnd
      setHasMore(nextEnd < bufferRef.current.length)
    } else {
      void loadGestores(cursorRef.current, true)
    }
  }, [hasMore, loadGestores])

  useEffect(() => {
    if (!enabled || !token) {
      setGestores([])
      setHasMore(true)
      cursorRef.current = 0
      return
    }
    cursorRef.current = 0
    setHasMore(true)
    void loadGestores(0, false)
  }, [token, codigoCliente, debouncedBusca, enabled, refreshTrigger, loadGestores])

  return {
    gestores,
    loading,
    loadingMore,
    error,
    hasMore,
    loadMore,
  }
}
