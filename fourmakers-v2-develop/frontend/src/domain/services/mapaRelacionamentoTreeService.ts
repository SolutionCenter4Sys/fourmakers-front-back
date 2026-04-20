// Domain Service: Mapa de Relacionamento Tree Operations
// Pure business logic for tree manipulation
// Moved from shared/utils/organogramaUtils.ts to follow Clean Architecture

import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

/**
 * Gera um ID único para um nó do mapa
 */
export function gerarIdNoMapa(): string {
  return `no_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
}

/**
 * Conta todos os descendentes de um nó recursivamente
 */
export function contarDescendentesMapa(no: NoMapaRelacionamento): number {
  if (no.children.length === 0) return 0
  return no.children.reduce((acc, filho) => acc + 1 + contarDescendentesMapa(filho), 0)
}

/**
 * Busca um nó por ID na árvore recursivamente
 */
export function buscarNoPorIdMapa(estrutura: NoMapaRelacionamento, id: string): NoMapaRelacionamento | null {
  if (estrutura.id === id) return estrutura

  for (const filho of estrutura.children) {
    const encontrado = buscarNoPorIdMapa(filho, id)
    if (encontrado) return encontrado
  }

  return null
}

/**
 * Remove um nó da árvore por ID
 * Retorna null se o nó raiz for removido
 */
export function removerNoMapa(estrutura: NoMapaRelacionamento, idRemover: string): NoMapaRelacionamento | null {
  if (estrutura.id === idRemover) {
    return null
  }

  return {
    ...estrutura,
    children: estrutura.children
      .map((filho) => removerNoMapa(filho, idRemover))
      .filter((filho): filho is NoMapaRelacionamento => filho !== null),
  }
}

/**
 * Atualiza um nó específico na árvore com novos dados
 */
export function atualizarNoMapa(
  estrutura: NoMapaRelacionamento,
  idAtualizar: string,
  dadosAtualizados: Partial<NoMapaRelacionamento>,
): NoMapaRelacionamento {
  if (estrutura.id === idAtualizar) {
    return { ...estrutura, ...dadosAtualizados }
  }

  return {
    ...estrutura,
    children: estrutura.children.map((filho) => atualizarNoMapa(filho, idAtualizar, dadosAtualizados)),
  }
}

/**
 * Adiciona um filho a um nó específico
 */
export function adicionarFilhoMapa(
  estrutura: NoMapaRelacionamento,
  idPai: string,
  novoFilho: NoMapaRelacionamento,
): NoMapaRelacionamento {
  if (estrutura.id === idPai) {
    return {
      ...estrutura,
      children: [...estrutura.children, novoFilho],
    }
  }

  return {
    ...estrutura,
    children: estrutura.children.map((filho) => adicionarFilhoMapa(filho, idPai, novoFilho)),
  }
}

/**
 * Move um nó de uma posição para outra na árvore
 */
export function moverNoMapa(
  estrutura: NoMapaRelacionamento,
  idOrigem: string,
  idDestino: string,
): NoMapaRelacionamento | null {
  const noOrigem = buscarNoPorIdMapa(estrutura, idOrigem)
  if (!noOrigem) return estrutura

  const novaEstrutura = removerNoMapa(estrutura, idOrigem)
  if (!novaEstrutura) return estrutura

  return adicionarFilhoMapa(novaEstrutura, idDestino, noOrigem)
}

/**
 * Função auxiliar que coleta todos os C-Levels recursivamente.
 * Ignora nós intermediários com isCLevel !== true e "promove" seus filhos C-Level.
 */
function coletarCLevelsRecursivo(noAtual: NoMapaRelacionamento): NoMapaRelacionamento[] {
  if (noAtual.isCLevel === true) {
    const filhosCLevels = noAtual.children.flatMap(coletarCLevelsRecursivo)
    return [{ ...noAtual, children: filhosCLevels }]
  }
  return noAtual.children.flatMap(coletarCLevelsRecursivo)
}

/**
 * Filtra a árvore para mostrar APENAS nós com `isCLevel === true`.
 * A posição do nó na hierarquia é irrelevante — somente o campo `isCLevel` determina a inclusão.
 *
 * - Nó virtual (`id === 'root-virtual'`): mantido como container estrutural; seus filhos são filtrados.
 * - Nó real C-Level: mantido com seus filhos C-Level.
 * - Nó real não-C-Level na raiz: seus C-Levels descendentes são coletados e agrupados num virtual root.
 * - Sem C-Levels encontrados: retorna null (tela exibe "Estrutura vazia").
 */
export function filtrarCLevelsMapa(no: NoMapaRelacionamento): NoMapaRelacionamento | null {
  // Nó virtual é estrutural (não representa pessoa): mantém como container
  if (no.id === 'root-virtual') {
    const filhosCLevels = no.children.flatMap(coletarCLevelsRecursivo)
    return { ...no, children: filhosCLevels }
  }

  const cLevels = coletarCLevelsRecursivo(no)

  if (cLevels.length === 0) return null
  if (cLevels.length === 1) return cLevels[0]

  // Root não era C-Level mas possui múltiplos C-Level descendentes: agrupa num virtual root
  return {
    id: 'root-virtual',
    profileId: null,
    employeeId: null,
    employeeName: 'Organograma',
    children: cLevels,
  }
}

/**
 * Converte a árvore em uma lista plana com informação de nível
 * Ignora o nó virtual "Organograma" (root-virtual) quando há múltiplas raízes
 */
export function converterParaListaMapa(no: NoMapaRelacionamento, nivel: number = 0): Array<NoMapaRelacionamento & { nivel: number }> {
  // Se for o nó virtual "Organograma", processar apenas seus filhos diretamente (sem incluir o nó virtual)
  if (no.id === 'root-virtual') {
    const resultado: Array<NoMapaRelacionamento & { nivel: number }> = []
    // Processar filhos como raízes (nível 0)
    no.children.forEach((filho) => {
      resultado.push(...converterParaListaMapa(filho, 0))
    })
    return resultado
  }

  const resultado: Array<NoMapaRelacionamento & { nivel: number }> = [{ ...no, nivel }]

  no.children.forEach((filho) => {
    resultado.push(...converterParaListaMapa(filho, nivel + 1))
  })

  return resultado
}

// Alias exports without "Mapa" suffix for cleaner imports
export const gerarIdNo = gerarIdNoMapa
export const contarDescendentes = contarDescendentesMapa
export const buscarNoPorId = buscarNoPorIdMapa
export const removerNo = removerNoMapa
export const atualizarNo = atualizarNoMapa
export const adicionarFilho = adicionarFilhoMapa
export const moverNo = moverNoMapa
export const filtrarCLevels = filtrarCLevelsMapa
export const converterParaLista = converterParaListaMapa
