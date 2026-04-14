import { useState, useEffect, useCallback } from 'react'
import { useAppSelector } from '@app/store/hooks'
import { container } from '@core/di/container'
import { DiTokens } from '@core/di/tokens'
import type { IntegracaoBancariaApi } from '@data/api/IntegracaoBancariaApi'
import type {
  RemessaCnab,
  ColaboradorSolicitacaoCNAB,
  StatusRemessa,
} from '@data/api/IntegracaoBancariaApi'

export const useIntegracaoBancaria = () => {
  const token = useAppSelector((state) => state.auth.token) || localStorage.getItem('authToken')
  
  const [solicitacoes, setSolicitacoes] = useState<ColaboradorSolicitacaoCNAB[]>([])
  const [remessas, setRemessas] = useState<RemessaCnab[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [modulosRemessa, setModulosRemessa] = useState<string[]>([])
  const [tipoRemessa, setTipoRemessa] = useState<string>('')

  // Carregar módulos de remessa disponíveis
  const loadModulosRemessa = useCallback(async () => {
    if (!token) {
      setError('Token não encontrado')
      return
    }

    try {
      console.log('[useIntegracaoBancaria] Carregando módulos de remessa')
      const api = container.resolve<IntegracaoBancariaApi>(DiTokens.integracaoBancariaApi)
      const response = await api.buscarModulosRemessa(token)

      console.log('[useIntegracaoBancaria] Resposta módulos:', response)

      if (response.sucesso && response.retorno) {
        console.log('[useIntegracaoBancaria] Módulos recebidos:', response.retorno)
        setModulosRemessa(response.retorno)
        
        // Se houver módulos e não houver tipo selecionado, selecionar o primeiro
        if (response.retorno.length > 0 && !tipoRemessa) {
          console.log('[useIntegracaoBancaria] Selecionando primeiro módulo:', response.retorno[0])
          setTipoRemessa(response.retorno[0])
        }
      } else {
        const errorMsg = response.mensagem || 'Erro ao carregar módulos de remessa'
        console.error('[useIntegracaoBancaria] Erro na resposta:', errorMsg)
        setModulosRemessa([])
      }
    } catch (err) {
      console.error('[useIntegracaoBancaria] Erro ao carregar módulos:', err)
      setModulosRemessa([])
    }
  }, [token, tipoRemessa])

  // Carregar solicitações aprovadas
  const loadSolicitacoes = useCallback(async (codDiretoria?: string, tipo?: string) => {
    if (!token) {
      setError('Token não encontrado')
      return
    }

    const tipoAtual = tipo || tipoRemessa
    
    // Só faz a busca se houver tipo de remessa selecionado
    if (!tipoAtual) {
      console.log('[useIntegracaoBancaria] Tipo de remessa não selecionado, pulando busca de solicitações')
      setSolicitacoes([])
      return
    }

    try {
      setError(null)
      console.log('[useIntegracaoBancaria] Carregando solicitações - codDiretoria:', codDiretoria || 'vazio', 'tipoRemessa:', tipoAtual)
      const api = container.resolve<IntegracaoBancariaApi>(DiTokens.integracaoBancariaApi)
      const response = await api.listarSolicitacoesPagamentoCNAB(token, {
        tipoRemessa: tipoAtual,
        codDiretoria: codDiretoria || undefined,
      })

      console.log('[useIntegracaoBancaria] Resposta completa:', response)
      console.log('[useIntegracaoBancaria] Sucesso:', response.sucesso)
      console.log('[useIntegracaoBancaria] Retorno:', response.retorno)

      if (response.sucesso) {
        const colaboradores = response.retorno?.colaboradores || []
        console.log('[useIntegracaoBancaria] Colaboradores recebidos:', colaboradores.length)
        console.log('[useIntegracaoBancaria] Total de solicitações:', response.retorno?.totalSolicitacoes || 0)
        setSolicitacoes(colaboradores)
        
        if (colaboradores.length === 0) {
          console.warn('[useIntegracaoBancaria] Nenhum colaborador retornado')
        }
      } else {
        const errorMsg = response.mensagem || 'Erro ao carregar solicitações'
        console.error('[useIntegracaoBancaria] Erro na resposta:', errorMsg)
        setError(errorMsg)
        setSolicitacoes([])
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao carregar solicitações'
      setError(errorMessage)
      setSolicitacoes([])
      console.error('[useIntegracaoBancaria] Erro ao carregar solicitações:', err)
    }
  }, [token, tipoRemessa])

  // Carregar remessas geradas
  const loadRemessas = useCallback(async (comp?: string, status?: StatusRemessa, tipo?: string) => {
    if (!token) {
      setError('Token não encontrado')
      return
    }

    const tipoAtual = tipo || tipoRemessa
    
    // Só faz a busca se houver tipo de remessa selecionado
    if (!tipoAtual) {
      console.log('[useIntegracaoBancaria] Tipo de remessa não selecionado, pulando busca de remessas')
      setRemessas([])
      return
    }

    try {
      setError(null)
      console.log('[useIntegracaoBancaria] Carregando remessas para competência:', comp || 'todas', 'status:', status || 'todos', 'tipo:', tipoAtual)
      const api = container.resolve<IntegracaoBancariaApi>(DiTokens.integracaoBancariaApi)
      const response = await api.listarRemessasCnab(token, {
        tipoRemessa: tipoAtual,
        ...(comp && { competencia: comp }),
        ...(status && { status }),
      })

      console.log('[useIntegracaoBancaria] Resposta remessas completa:', JSON.stringify(response, null, 2))
      console.log('[useIntegracaoBancaria] response.retorno:', response.retorno)
      console.log('[useIntegracaoBancaria] response.retorno?.remessas:', response.retorno?.remessas)
      console.log('[useIntegracaoBancaria] Tipo de response.retorno:', typeof response.retorno)
      console.log('[useIntegracaoBancaria] response.retorno é array?', Array.isArray(response.retorno))

      if (response.sucesso) {
        // Verificar se retorno é um array (estrutura alternativa)
        let remessasData: RemessaCnab[] = []
        
        if (Array.isArray(response.retorno)) {
          // Se retorno é um array direto
          remessasData = response.retorno
          console.log('[useIntegracaoBancaria] Retorno é array direto, remessas:', remessasData.length)
        } else if (response.retorno?.remessas) {
          // Se retorno tem propriedade remessas
          remessasData = response.retorno.remessas
          console.log('[useIntegracaoBancaria] Retorno tem propriedade remessas, quantidade:', remessasData.length)
        } else {
          console.warn('[useIntegracaoBancaria] Estrutura de retorno não reconhecida:', response.retorno)
        }
        
        console.log('[useIntegracaoBancaria] Remessas recebidas:', remessasData.length)
        console.log('[useIntegracaoBancaria] Dados das remessas:', remessasData)
        setRemessas(remessasData)
        
        if (remessasData.length === 0) {
          console.warn('[useIntegracaoBancaria] Nenhuma remessa retornada')
        }
      } else {
        const errorMsg = response.mensagem || 'Erro ao carregar remessas'
        console.error('[useIntegracaoBancaria] Erro na resposta:', errorMsg)
        setError(errorMsg)
        setRemessas([])
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao carregar remessas'
      setError(errorMessage)
      setRemessas([])
      console.error('[useIntegracaoBancaria] Erro ao carregar remessas:', err)
    }
  }, [token, tipoRemessa])

  // Carregar módulos de remessa ao iniciar
  useEffect(() => {
    if (token) {
      loadModulosRemessa()
    }
  }, [token, loadModulosRemessa])

  // Carregar dados iniciais quando houver tipo de remessa
  useEffect(() => {
    const loadData = async () => {
      if (!token) {
        console.warn('[useIntegracaoBancaria] Token não encontrado')
        setError('Token não encontrado. Faça login novamente.')
        setLoading(false)
        return
      }

      if (!tipoRemessa) {
        console.log('[useIntegracaoBancaria] Aguardando seleção de tipo de remessa')
        setLoading(false)
        return
      }

      console.log('[useIntegracaoBancaria] Carregando dados para tipo:', tipoRemessa)
      setLoading(true)
      setError(null)
      
      // Executar apenas remessas no carregamento inicial
      // Solicitações serão carregadas pelo componente quando houver diretoria selecionada
      const results = await Promise.allSettled([
        loadRemessas(undefined, undefined, tipoRemessa), // Sem competência para mostrar todas as remessas
      ])
      
      // Verificar se alguma falhou
      const errors = results
        .filter((r) => r.status === 'rejected')
        .map((r) => r.status === 'rejected' ? r.reason : null)
        .filter(Boolean)
      
      if (errors.length > 0) {
        console.error('[useIntegracaoBancaria] Erros ao carregar dados:', errors)
        // Não definir erro global aqui, pois cada função já define seu próprio erro
      } else {
        console.log('[useIntegracaoBancaria] Dados carregados com sucesso')
      }
      
      setLoading(false)
    }

    loadData()
  }, [token, tipoRemessa, loadSolicitacoes, loadRemessas])

  // Carregar apenas solicitações quando competência mudar (sem recarregar remessas)
  // Nota: Este useEffect não carrega mais automaticamente
  // A diretoria é obrigatória e é gerenciada no componente
  // O componente chama loadSolicitacoes quando necessário

  // Processar remessa
  const processarRemessa = useCallback(async (
    codDiretoria: string | null,
    solicitacoesPagamentoIds: string[],
    tipo?: string
  ) => {
    if (!token) {
      throw new Error('Token não encontrado')
    }

    const tipoAtual = tipo || tipoRemessa

    const api = container.resolve<IntegracaoBancariaApi>(DiTokens.integracaoBancariaApi)
    const response = await api.processarRemessaBancariaCNAB(token, {
      tipoRemessa: tipoAtual,
      codDiretoria,
      solicitacoesPagamentoIds,
    })

    if (!response.sucesso) {
      throw new Error(response.mensagem || 'Erro ao processar remessa')
    }

    // Recarregar remessas após processar (todas as remessas)
    await loadRemessas(undefined, undefined, tipoAtual)
    
    return response.retorno.remessas // Retorna todas as remessas criadas
  }, [token, tipoRemessa, loadRemessas])

  // Processar retorno
  const processarRetorno = useCallback(async (
    file: File,
    hashRemessa: string,
    tipo?: string
  ) => {
    if (!token) {
      throw new Error('Token não encontrado')
    }

    const tipoAtual = tipo || tipoRemessa

    const api = container.resolve<IntegracaoBancariaApi>(DiTokens.integracaoBancariaApi)
    const response = await api.processarRetornoCnab(token, {
      file,
      hashRemessa,
      tipoRemessa: tipoAtual,
    })

    if (!response.sucesso) {
      throw new Error(response.mensagem || 'Erro ao processar retorno')
    }

    // Recarregar remessas após processar retorno (todas as remessas)
    await loadRemessas(undefined, undefined, tipoAtual)
    
    return response.retorno
  }, [token, tipoRemessa, loadRemessas])

  return {
    solicitacoes,
    remessas,
    loading,
    error,
    modulosRemessa,
    tipoRemessa,
    setTipoRemessa,
    loadModulosRemessa,
    loadSolicitacoes,
    loadRemessas,
    processarRemessa,
    processarRetorno,
    refresh: () => {
      if (token && tipoRemessa) {
        setLoading(true)
        // Apenas recarrega remessas. Solicitações devem ser recarregadas pelo componente
        // pois dependem do estado diretoriaSelecionada
        loadRemessas(undefined, undefined, tipoRemessa)
          .finally(() => setLoading(false))
      }
    },
  }
}

