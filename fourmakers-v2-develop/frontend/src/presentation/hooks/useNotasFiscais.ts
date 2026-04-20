import { useState, useEffect, useMemo } from 'react'
import { container } from '@core/di/container'
import { GetNotasFiscaisUseCase } from '@domain/usecases/GetNotasFiscaisUseCase'
import { useAppSelector } from '@app/store/hooks'
import type {
  NotaFiscalStatus,
  Unidade,
  ColaboradorMapaAlocacao,
  ListarNotasFiscaisPorVigenciaVisaoGestorParams,
  NotaFiscalPorVigencia,
  ListarRubricasColaboradorParaLiberacaoDeNfParams,
  RubricaColaborador,
  InserirNotaFiscalParams,
  InserirNotaFiscalResponse,
} from '@domain/entities/NotaFiscalGestao'
import type { NotaFiscalMock } from '@data/mocks/notasFiscaisMock'

export const useNotasFiscais = () => {
  const [notasFiscais, setNotasFiscais] = useState<NotaFiscalMock[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadNotasFiscais = async () => {
      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)
        const data = await useCase.execute()
        setNotasFiscais(data)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar notas fiscais')
      } finally {
        setLoading(false)
      }
    }

    loadNotasFiscais()
  }, [])

  return { notasFiscais, loading, error }
}

export const useNotasFiscaisGestao = () => {
  const { token } = useAppSelector((state) => state.auth)
  const [status, setStatus] = useState<NotaFiscalStatus[]>([])
  const [unidades, setUnidades] = useState<Unidade[]>([])
  const [colaboradores, setColaboradores] = useState<ColaboradorMapaAlocacao[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      if (!token) {
        setError('Token não encontrado')
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)

        // Carregar dados reais da API
        const [statusResponse, unidadesResponse, colaboradoresResponse] = await Promise.all([
          useCase.executeListarStatus(token),
          useCase.executeListarUnidades(token),
          useCase.executeListarColaboradoresETbds(token, {
            codigoDiretoria: 0,
            codigoGestor: 0,
            filtroTipoProfissional: 0,
            codigoDepartamento: '',
          }),
        ])

        // Processar status
        let statusData: NotaFiscalStatus[] = []
        if (Array.isArray(statusResponse)) {
          statusData = statusResponse.map((s: any) => ({
            id: s.id || s.Id || s.codigo || s.Codigo || 0,
            nome: s.nome || s.Nome || s.descricao || s.Descricao || s.descricaoStatus || s.DescricaoStatus || '',
            descricao: s.descricao || s.Descricao,
          }))
        } else if (statusResponse.sucesso) {
          const rawData = statusResponse.retorno || []
          statusData = rawData.map((s: any) => ({
            id: s.id || s.Id || s.codigo || s.Codigo || 0,
            nome: s.nome || s.Nome || s.descricao || s.Descricao || s.descricaoStatus || s.DescricaoStatus || '',
            descricao: s.descricao || s.Descricao,
          }))
        }
        setStatus(statusData)

        // Processar unidades
        let unidadesData: Unidade[] = []
        
        // Verificar se é array direto
        if (Array.isArray(unidadesResponse)) {
          unidadesData = unidadesResponse
            .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
            .map((u: any) => ({
              id: String(u.id || u.Id || ''),
              descricao: u.descricao || u.Descricao || '',
            }))
        } 
        // Verificar se tem estrutura de resposta com sucesso
        else if (unidadesResponse && typeof unidadesResponse === 'object') {
          // Verificar se tem ListaUnidadesResult (estrutura específica da API)
          if ('ListaUnidadesResult' in unidadesResponse && Array.isArray(unidadesResponse.ListaUnidadesResult)) {
            unidadesData = unidadesResponse.ListaUnidadesResult
              .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
              .map((u: any) => ({
                id: String(u.id || u.Id || ''),
                descricao: u.descricao || u.Descricao || '',
              }))
          }
          // Verificar se tem retorno direto
          else if ('retorno' in unidadesResponse) {
            const rawData = unidadesResponse.retorno || []
            unidadesData = Array.isArray(rawData)
              ? rawData
                  .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
                  .map((u: any) => ({
                    id: String(u.id || u.Id || ''),
                    descricao: u.descricao || u.Descricao || '',
                  }))
              : []
          }
          // Se é objeto direto com array dentro, buscar qualquer array
          else {
            const possibleArray = Object.values(unidadesResponse).find((val: any) => Array.isArray(val))
            if (possibleArray) {
              unidadesData = (possibleArray as any[])
                .filter((u: any) => u && (u.id || u.Id) && (u.descricao || u.Descricao))
                .map((u: any) => ({
                  id: String(u.id || u.Id || ''),
                  descricao: u.descricao || u.Descricao || '',
                }))
            }
          }
        }
        
        setUnidades(unidadesData)

        // Processar colaboradores (combinar colaboradores e TBDs)
        let todosColaboradores: ColaboradorMapaAlocacao[] = []
        
        const normalizeColaborador = (c: any): ColaboradorMapaAlocacao => ({
          codProfissional: c.codProfissional || c.CodProfissional || '',
          nomeProfissional: c.nomeProfissional || c.NomeProfissional || '',
          labelCodigoNome: c.labelCodigoNome || c.LabelCodigoNome || '',
          cpf: c.cpf || c.Cpf || c.CPF || '',
          ehTbd: c.ehTbd || c.EhTbd || c.EhTBD || false,
        })
        
        if (Array.isArray(colaboradoresResponse)) {
          todosColaboradores = colaboradoresResponse.map(normalizeColaborador)
        } else if (colaboradoresResponse.sucesso) {
          if (colaboradoresResponse.retorno) {
            // Pode ser um objeto com colaboradores e tbds, ou um array direto
            if (Array.isArray(colaboradoresResponse.retorno)) {
              todosColaboradores = colaboradoresResponse.retorno.map(normalizeColaborador)
            } else {
              const colaboradores = (colaboradoresResponse.retorno.colaboradores || []).map(normalizeColaborador)
              const tbds = (colaboradoresResponse.retorno.tbds || []).map(normalizeColaborador)
              todosColaboradores = [...colaboradores, ...tbds]
            }
          }
        }
        
        setColaboradores(todosColaboradores)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar dados')
      } finally {
        setLoading(false)
      }
    }

    loadData()
  }, [token])

  return {
    status,
    unidades,
    colaboradores,
    loading,
    error,
  }
}

export const useNotasFiscaisPorVigencia = (params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams) => {
  const { token } = useAppSelector((state) => state.auth)
  const [notasFiscais, setNotasFiscais] = useState<NotaFiscalPorVigencia[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [refetchKey, setRefetchKey] = useState(0)

  useEffect(() => {
    const loadNotasFiscais = async () => {
      if (!token) {
        setError('Token não encontrado')
        setLoading(false)
        return
      }

      // Não fazer busca se não houver parâmetros
      if (!params) {
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)
        const response = await useCase.executeListarNotasFiscaisPorVigenciaVisaoGestor(token, params)

        if (response.sucesso && response.retorno) {
          setNotasFiscais(response.retorno)
        } else {
          setError(response.mensagem || 'Erro ao carregar notas fiscais')
          setNotasFiscais([])
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar notas fiscais')
        setNotasFiscais([])
      } finally {
        setLoading(false)
      }
    }

    loadNotasFiscais()
  }, [token, params, refetchKey])

  const refetch = () => setRefetchKey((k) => k + 1)

  return {
    notasFiscais,
    loading,
    error,
    refetch,
  }
}

/**
 * Hook para buscar notas fiscais por vigência (visão colaborador)
 */
export const useNotasFiscaisPorVigenciaColaborador = (params?: ListarNotasFiscaisPorVigenciaVisaoGestorParams) => {
  const { token } = useAppSelector((state) => state.auth)
  const [notasFiscais, setNotasFiscais] = useState<NotaFiscalPorVigencia[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Serializar params para usar como dependência estável
  const paramsKey = useMemo(() => {
    if (!params) return null
    return JSON.stringify({
      mes: params.mes,
      ano: params.ano,
      statusId: params.statusId,
      cursor: params.cursor,
      limite: params.limite,
    })
  }, [params?.mes, params?.ano, params?.statusId, params?.cursor, params?.limite])

  useEffect(() => {
    const loadNotasFiscais = async () => {
      if (!token) {
        setError('Token não encontrado')
        setLoading(false)
        return
      }

      // Não fazer busca se não houver mes e ano
      if (!params || !params.mes || !params.ano) {
        setNotasFiscais([])
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)
        const response = await useCase.executeListarNotasFiscaisPorVigencia(token, params)

        if (response.sucesso && response.retorno) {
          setNotasFiscais(response.retorno)
        } else {
          setError(response.mensagem || 'Erro ao carregar notas fiscais')
          setNotasFiscais([])
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar notas fiscais')
        setNotasFiscais([])
      } finally {
        setLoading(false)
      }
    }

    loadNotasFiscais()
  }, [token, paramsKey])

  return {
    notasFiscais,
    loading,
    error,
  }
}

/**
 * Hook para buscar apenas os status de notas fiscais
 */
export const useNotaFiscalStatus = () => {
  const { token } = useAppSelector((state) => state.auth)
  const [status, setStatus] = useState<NotaFiscalStatus[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const loadStatus = async () => {
      if (!token) {
        setError('Token não encontrado')
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)
        const response = await useCase.executeListarStatus(token)

        // Processar status - a API retorna { id, descricao }
        let statusData: NotaFiscalStatus[] = []
        if (response.sucesso && response.retorno) {
          const rawData = response.retorno || []
          statusData = rawData.map((s: any) => ({
            id: s.id || s.Id || 0,
            nome: s.descricao || s.Descricao || '', // API retorna 'descricao', mapeamos para 'nome'
            descricao: s.descricao || s.Descricao,
          }))
        } else if (Array.isArray(response)) {
          // Fallback caso a resposta seja um array direto
          statusData = response.map((s: any) => ({
            id: s.id || s.Id || 0,
            nome: s.descricao || s.Descricao || '',
            descricao: s.descricao || s.Descricao,
          }))
        } else {
          setError(response.mensagem || 'Erro ao carregar status')
        }
        
        setStatus(statusData)
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar status')
        setStatus([])
      } finally {
        setLoading(false)
      }
    }

    loadStatus()
  }, [token])

  return {
    status,
    loading,
    error,
  }
}

/**
 * Hook para buscar rubricas do colaborador para liberação de NF
 */
export const useRubricasColaboradorParaLiberacaoDeNf = (params?: ListarRubricasColaboradorParaLiberacaoDeNfParams) => {
  const { token } = useAppSelector((state) => state.auth)
  const [rubricas, setRubricas] = useState<RubricaColaborador[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Serializar params para usar como dependência estável
  const paramsKey = useMemo(() => {
    if (!params) return null
    return JSON.stringify({
      mes: params.mes,
      ano: params.ano,
    })
  }, [params?.mes, params?.ano])

  useEffect(() => {
    const loadRubricas = async () => {
      if (!token) {
        setError('Token não encontrado')
        setLoading(false)
        return
      }

      // Não fazer busca se não houver mes e ano
      if (!params || !params.mes || !params.ano) {
        setRubricas([])
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        setError(null)
        const useCase = container.resolve(GetNotasFiscaisUseCase)
        const response = await useCase.executeListarRubricasColaboradorParaLiberacaoDeNf(token, params)

        if (response.sucesso && response.retorno) {
          setRubricas(response.retorno)
        } else {
          setError(response.mensagem || 'Erro ao carregar rubricas')
          setRubricas([])
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Erro ao carregar rubricas')
        setRubricas([])
      } finally {
        setLoading(false)
      }
    }

    loadRubricas()
  }, [token, paramsKey])

  return {
    rubricas,
    loading,
    error,
  }
}

/**
 * Hook para inserir nota fiscal
 */
export const useInserirNotaFiscal = () => {
  const { token } = useAppSelector((state) => state.auth)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const inserirNotaFiscal = async (params: InserirNotaFiscalParams): Promise<InserirNotaFiscalResponse> => {
    if (!token) {
      throw new Error('Token não encontrado')
    }

    try {
      setLoading(true)
      setError(null)
      const useCase = container.resolve(GetNotasFiscaisUseCase)
      const response = await useCase.executeInserirNotaFiscal(token, params)

      if (!response.sucesso) {
        const errorMessage = response.mensagem || response.erros?.join(', ') || 'Erro ao inserir nota fiscal'
        setError(errorMessage)
        throw new Error(errorMessage)
      }

      return response
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao inserir nota fiscal'
      setError(errorMessage)
      throw err
    } finally {
      setLoading(false)
    }
  }

  return {
    inserirNotaFiscal,
    loading,
    error,
  }
}

