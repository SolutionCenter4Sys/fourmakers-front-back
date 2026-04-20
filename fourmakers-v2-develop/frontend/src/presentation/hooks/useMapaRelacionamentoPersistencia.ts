import { useEffect } from 'react'

import { useAppDispatch, useAppSelector } from '@app/store/hooks'
import { setEstruturaMapa } from '@app/store/slices/mapaRelacionamentoSlice'
import type { DadosMapaRelacionamentoArmazenados } from '@domain/entities/MapaRelacionamento'
import { MAPA_RELACIONAMENTO_CONSTANTS } from '@shared/constants/mapaRelacionamentoConstants'
import { getMockTreeForClient, MOCK_CLIENTES_MAPA_RELACIONAMENTO } from '@shared/mocks/mapaRelacionamentoMock'

export function useMapaRelacionamentoPersistencia() {
  const dispatch = useAppDispatch()
  const { codigoClienteSelecionado, estruturaMapa } = useAppSelector((state) => state.mapaRelacionamento)

  // Load from localStorage when client changes
  useEffect(() => {
    if (!codigoClienteSelecionado) {
      dispatch(setEstruturaMapa(null))
      return
    }

    const chave = `${MAPA_RELACIONAMENTO_CONSTANTS.STORAGE_PREFIX}${codigoClienteSelecionado}`
    const dadosSalvos = localStorage.getItem(chave)

    if (dadosSalvos) {
      try {
        const dados: DadosMapaRelacionamentoArmazenados = JSON.parse(dadosSalvos)
        // No PROTOTIPO, o objeto salvo é o próprio nó raiz com clientName
        // Verificar se tem estrutura ou se é o nó raiz direto
        const estrutura = dados.estrutura || (dados.id ? dados : null)
        if (estrutura) {
          dispatch(setEstruturaMapa(estrutura as any))
        } else {
          dispatch(setEstruturaMapa(null))
        }
      } catch (error) {
        console.error('Erro ao carregar mapa de relacionamento do localStorage:', error)
        dispatch(setEstruturaMapa(null))
      }
    } else {
      // No localStorage data - check if this is a mock client
      const isMockClient = MOCK_CLIENTES_MAPA_RELACIONAMENTO.some((c) => c.codigoCliente === codigoClienteSelecionado) || codigoClienteSelecionado.startsWith('MOCK')
      if (isMockClient) {
        const mockTree = getMockTreeForClient(codigoClienteSelecionado)
        dispatch(setEstruturaMapa(mockTree))
      } else {
        dispatch(setEstruturaMapa(null))
      }
    }
  }, [codigoClienteSelecionado, dispatch])

  // Save to localStorage when structure changes (debounced)
  useEffect(() => {
    if (!codigoClienteSelecionado || !estruturaMapa) {
      return
    }

    const timeoutId = setTimeout(() => {
      const chave = `${MAPA_RELACIONAMENTO_CONSTANTS.STORAGE_PREFIX}${codigoClienteSelecionado}`
      // No PROTOTIPO, salva o nó raiz diretamente com clientName
      const dados: DadosMapaRelacionamentoArmazenados = {
        ...estruturaMapa,
        codigoCliente: codigoClienteSelecionado,
        dataUltimaAtualizacao: new Date().toISOString(),
      }

      try {
        localStorage.setItem(chave, JSON.stringify(dados))
      } catch (error) {
        console.error('Erro ao salvar mapa de relacionamento no localStorage:', error)
      }
    }, MAPA_RELACIONAMENTO_CONSTANTS.SAVE_DEBOUNCE_MS)

    return () => clearTimeout(timeoutId)
  }, [codigoClienteSelecionado, estruturaMapa])
}
