import type { PosicaoCompleta } from '@domain/entities/Organograma'
import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'
import { EMPTY_GUID } from '@shared/utils/mapaRelacionamentoUtils'

/**
 * Converte uma lista plana de posições em uma estrutura hierárquica de nós
 * Baseado no campo posicaoIdSuperior para construir a hierarquia
 * Valores null são tratados como "Vago" conforme regras do IMPLEMENTATION.md
 */
export function converterPosicoesParaArvore(
  posicoes: PosicaoCompleta[],
): NoMapaRelacionamento | null {
  if (posicoes.length === 0) return null

  // Filtrar apenas posições ativas (ativo === true)
  const posicoesAtivas = posicoes.filter((pos) => pos.ativo === true)
  
  if (posicoesAtivas.length === 0) return null

  // Criar mapa de posições por ID para acesso rápido
  const posicoesMap = new Map<string, PosicaoCompleta>()
  posicoesAtivas.forEach((pos) => {
    if (pos.id) {
      posicoesMap.set(pos.id, pos)
    }
  })

  // Converter cada posição em nó
  const nosMap = new Map<string, NoMapaRelacionamento>()

  posicoesAtivas.forEach((pos) => {
    // Buscar a primeira alocação ativa (se houver)
    // IMPORTANTE: Tratar casos de alocacoes: [] (array vazio) e nomeColaborador null
    const alocacaoAtiva = pos.alocacoes?.find((aloc) => aloc.ativo && !aloc.dataFim) || pos.alocacoes?.[0]
    
    // Verificar se é vaga:
    // 1. Se não há alocacoes ou array está vazio (alocacaoAtiva será undefined)
    // 2. Se alocacaoAtiva existe mas nomeColaborador é null/undefined/vazio
    // 3. Se alocacaoAtiva existe mas codigoInternoColaborador é null/undefined/vazio
    // 4. Se codigoInternoColaborador é o GUID vazio (00000000-0000-0000-0000-000000000000)
    const hasValidAlocacao = !!(
      alocacaoAtiva && 
      alocacaoAtiva.nomeColaborador && 
      typeof alocacaoAtiva.nomeColaborador === 'string' &&
      alocacaoAtiva.nomeColaborador.trim() !== '' &&
      alocacaoAtiva.codigoInternoColaborador && 
      typeof alocacaoAtiva.codigoInternoColaborador === 'string' &&
      alocacaoAtiva.codigoInternoColaborador.trim() !== '' &&
      alocacaoAtiva.codigoInternoColaborador !== EMPTY_GUID
    )
    
    // Tratar valores null/vazios como "Vago" conforme regras
    const employeeName = hasValidAlocacao ? alocacaoAtiva.nomeColaborador : 'Vago'
    const employeeId = hasValidAlocacao ? alocacaoAtiva.codigoInternoColaborador : 'vacant'
    const profileName = pos.perfilCorporativoNome || undefined
    const departmentName = pos.departamentoNome || 'Sem Departamento'

    const no: NoMapaRelacionamento = {
      id: pos.id,
      posicaoId: pos.id, // ID da posição no backend
      profileId: pos.perfilCorporativoId || null, // Pode ser gestorExternoPerfilId ou perfilCorporativoId
      employeeId: employeeId,
      profileName: profileName,
      employeeName: employeeName,
      employeeEmail: undefined, // Not available in new structure
      employeeAvatarUrl: undefined, // Not available in new structure
      departmentId: pos.departamentoId || undefined, // Usar departamentoId do backend como cod do dropdown
      departmentName: departmentName,
      isCLevel: pos.cLevel,
      isExternal: pos.profissionalExterno,
      wasConnected: hasValidAlocacao,
      alocacaoId: hasValidAlocacao ? (alocacaoAtiva.id || undefined) : undefined,
      departamentoBackendId: pos.departamentoId || undefined,
      perfilCorporativoBackendId: pos.perfilCorporativoId || undefined,
      children: [],
    }
    nosMap.set(pos.id, no)
  })

  // Construir hierarquia usando posicaoIdSuperior
  const raiz: NoMapaRelacionamento[] = []

  nosMap.forEach((no, posicaoId) => {
    const posicao = posicoesMap.get(posicaoId)
    if (!posicao) return

    const paiId = posicao.posicaoIdSuperior

    if (!paiId) {
      // Nó raiz (sem pai)
      raiz.push(no)
    } else {
      // Nó filho - adicionar ao pai apenas se o pai também estiver ativo
      const pai = nosMap.get(paiId)
      if (pai) {
        pai.children.push(no)
      } else {
        // Pai não encontrado ou inativo - tratar como raiz (caso de dados inconsistentes ou pai inativo)
        raiz.push(no)
      }
    }
  })

  // Se houver apenas uma raiz, retornar ela
  // Se houver múltiplas raízes, criar um nó raiz virtual invisível
  // O nó virtual será completamente invisível na visualização (sem nó pai visível)
  // e seus filhos (as raízes reais) aparecerão lado a lado no topo
  if (raiz.length === 1) {
    return raiz[0]
  } else if (raiz.length > 1) {
    // Criar nó raiz virtual que contém todas as raízes
    // Este nó será invisível na visualização, então não haverá nó pai visível
    return {
      id: 'root-virtual',
      profileId: null,
      employeeId: null,
      employeeName: 'Organograma',
      children: raiz,
    }
  }

  return null
}
