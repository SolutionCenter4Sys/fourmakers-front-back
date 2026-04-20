import type { NoMapaRelacionamento } from '@domain/entities/MapaRelacionamento'

/**
 * Interface para o formato de nó do react-d3-tree
 * Representa um nó da árvore de relacionamentos no formato esperado pela biblioteca D3
 */
export interface D3TreeNode {
  name: string
  attributes: {
    nodeData: string
    id: string
    profileId: string | null
    employeeId: string | null
    profileName: string | null
    employeeEmail: string | null
    departmentName: string | null
    isCLevel: boolean
    isExternal: boolean
    wasConnected: boolean
    hasOriginalChildren: boolean
    originalChildrenCount: number
  }
  children: D3TreeNode[]
}

/**
 * Helper to get all nodes from tree recursively
 * Retorna uma lista plana de todos os nós da árvore
 */
export function getAllNodes(node: NoMapaRelacionamento): NoMapaRelacionamento[] {
  const result = [node]
  node.children.forEach((child) => {
    result.push(...getAllNodes(child))
  })
  return result
}

/**
 * Helper to count all descendants recursively
 * Conta todos os descendentes de um nó (filhos + netos + bisnetos + ...)
 */
export function countAllDescendants(node: NoMapaRelacionamento): number {
  let count = 0
  if (node.children) {
    count += node.children.length
    node.children.forEach((child) => {
      count += countAllDescendants(child)
    })
  }
  return count
}

/**
 * Convert NoMapaRelacionamento to react-d3-tree format
 * Converte a estrutura de nós do domínio para o formato esperado pela biblioteca D3
 * 
 * @param node - Nó atual (potencialmente filtrado)
 * @param allNodes - Todos os nós da árvore original (não filtrada)
 * @param originalNode - Nó original antes de filtros (preserva informação de filhos)
 * @returns Nó no formato D3TreeNode
 * 
 * originalNode is used to preserve original children info even when node is filtered
 */
export function convertToD3Tree(
  node: NoMapaRelacionamento,
  allNodes: NoMapaRelacionamento[],
  originalNode?: NoMapaRelacionamento,
): D3TreeNode {
  // Use originalNode if provided, otherwise use current node
  // This ensures we always have the original children information
  const sourceNode = originalNode || node
  const hasOriginalChildren = sourceNode.children && sourceNode.children.length > 0

  return {
    name: node.employeeName || 'Vago',
    attributes: {
      // Store full ORIGINAL node data as JSON for reconstruction
      // This is critical - we store the original node, not the filtered one
      // allNodes is provided via MapaRelacionamentoTreeContext to avoid O(n²) serialization
      nodeData: JSON.stringify(sourceNode),
      id: node.id,
      profileId: node.profileId,
      employeeId: node.employeeId,
      profileName: node.profileName ?? null,
      employeeEmail: node.employeeEmail ?? null,
      departmentName: node.departmentName ?? null,
      isCLevel: node.isCLevel ?? false,
      isExternal: node.isExternal ?? false,
      wasConnected: node.wasConnected ?? false,
      hasOriginalChildren: hasOriginalChildren,
      originalChildrenCount: sourceNode.children?.length || 0,
    },
    // Use filtered node's children for rendering (may be empty if collapsed)
    // But we map each child with its original reference
    children: node.children.map((child) => {
      // Find the corresponding original child to preserve its full structure
      const originalChild = sourceNode.children?.find((c) => c.id === child.id)
      // Pass the original child so it can preserve its own children info
      return convertToD3Tree(child, allNodes, originalChild)
    }),
  }
}
