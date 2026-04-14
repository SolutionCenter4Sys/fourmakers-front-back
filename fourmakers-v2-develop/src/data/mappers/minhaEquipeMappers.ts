import type { MinhaEquipeColaborador } from '@domain/entities/MinhaEquipeColaborador'
import type { MinhaEquipeKPIs } from '@domain/entities/MinhaEquipeKPIs'
import type { MinhaEquipeSugestao } from '@domain/entities/MinhaEquipeSugestao'
import type { MinhaEquipePDI } from '@domain/entities/MinhaEquipePDI'
import type { MinhaEquipeHabilidade } from '@domain/entities/MinhaEquipeHabilidade'
import type {
  ListaIndicadoresDetalhamentoCalculo,
  ListaIndicadoresLideradosAgrupadoItem,
  ListaIndicadoresLideradosLegacyItem,
  ListaIndicadoresLideradosResponse,
  ListaIndicadoresRawHabilidade,
  TotalizacaoIndicadoresResponse,
  BuscarSugestoesResponse,
  XanoPDIResponse,
} from '@data/api/MinhaEquipeApi'

/**
 * Mapeia resposta da API para lista de colaboradores
 * Suporta a estrutura legacy e a nova resposta agrupada por gestores/clientes.
 */
export const mapColaboradoresFromApi = (
  response: ListaIndicadoresLideradosResponse
): MinhaEquipeColaborador[] => {
  if (!response?.retorno) return []

  const colaboradores: MinhaEquipeColaborador[] = []

  for (const item of response.retorno) {
    if (isAgrupadoItem(item)) {
      colaboradores.push(...mapAgrupadoItemToColaboradores(item))
      continue
    }

    colaboradores.push(mapLegacyItemToColaborador(item))
  }

  return colaboradores
}

const isAgrupadoItem = (
  item: ListaIndicadoresLideradosResponse['retorno'][number]
): item is ListaIndicadoresLideradosAgrupadoItem =>
  typeof item === 'object' &&
  item !== null &&
  Array.isArray((item as ListaIndicadoresLideradosAgrupadoItem).colaboradores)

const buildGestoresOperacionaisFromItem = (
  item: ListaIndicadoresLideradosAgrupadoItem
): string[] => {
  const nomes = new Set<string>()

  if (item.nomeGestorOperacional) {
    nomes.add(item.nomeGestorOperacional)
  }

  item.gestoresOperacionais?.forEach((gestor) => {
    if (gestor.nomeGestorOperacional) {
      nomes.add(gestor.nomeGestorOperacional)
    }
  })

  return Array.from(nomes)
}

const mapAgrupadoItemToColaboradores = (
  item: ListaIndicadoresLideradosAgrupadoItem
): MinhaEquipeColaborador[] => {
  const gestoresOperacionais = buildGestoresOperacionaisFromItem(item)
  const nomeGestorOperacional =
    item.nomeGestorOperacional || gestoresOperacionais[0] || ''
  const nomeGestorAdm = item.nomeGestorAdm || ''
  const codigoGestorAdm = item.codigoGestorAdm || ''

  const colaboradores: MinhaEquipeColaborador[] = []

  for (const colaborador of item.colaboradores ?? []) {
    if (!colaborador) continue

    const clientes = Array.isArray(colaborador.clientes)
      ? colaborador.clientes
      : []

    if (clientes.length === 0) {
      colaboradores.push({
        id: `${colaborador.codigoInternoColaborador}-sem-cliente`,
        nomeColaborador: colaborador.nomeColaborador,
        nomeCliente: 'Sem cliente ativo',
        nomeGestorCliente: '',
        perfil: '',
        perfilId: '',
        nomeGestorOperacional,
        nomeGestorAdm,
        codigoInternoColaborador: colaborador.codigoInternoColaborador,
        codigoGestorAdm,
        idAlocacao: colaborador.idAlocacao ?? 0,
        match: 0,
        retornoMatch: createEmptyRetornoMatch(),
        resultadoHabilidades: null,
        gestoresOperacionais,
      })
      continue
    }

    clientes.forEach((cliente, index) => {
      const clientSegment =
        cliente.codigoCliente ||
        cliente.perfilId ||
        `cliente-${index + 1}`
      const rowId = `${colaborador.codigoInternoColaborador}-${clientSegment}-${index}`

      colaboradores.push({
        id: rowId,
        nomeColaborador: colaborador.nomeColaborador,
        nomeCliente: cliente.nomeCliente ?? 'Sem cliente ativo',
        nomeGestorCliente: cliente.nomeGestorCliente ?? '',
        perfil: cliente.perfil ?? '',
        perfilId: cliente.perfilId ?? '',
        nomeGestorOperacional,
        nomeGestorAdm,
        codigoInternoColaborador: colaborador.codigoInternoColaborador,
        codigoGestorAdm,
        idAlocacao: colaborador.idAlocacao ?? 0,
        match: cliente.retornoMatch?.match ?? 0,
        retornoMatch: mapDetalhamentoCalculoToRetornoMatch(
          cliente.retornoMatch?.detalhamento_calculo
        ),
        resultadoHabilidades: mapResultadoHabilidadesFromApi(
          cliente.resultadoHabilidades ?? null,
          rowId
        ),
        gestoresOperacionais,
      })
    })
  }

  return colaboradores
}

const mapLegacyItemToColaborador = (
  item: ListaIndicadoresLideradosLegacyItem
): MinhaEquipeColaborador => ({
  id: item.id || item.codigoInternoColaborador,
  nomeColaborador: item.nomeColaborador,
  nomeCliente: item.nomeCliente,
  nomeGestorCliente: item.nomeGestorCliente,
  perfil: item.perfil,
  perfilId: item.perfilId,
  nomeGestorOperacional: item.nomeGestorOperacional,
  nomeGestorAdm: item.nomeGestorAdm,
  codigoInternoColaborador: item.codigoInternoColaborador,
  codigoGestorAdm: item.codigoGestorAdm,
  idAlocacao: item.idAlocacao,
  match: item.match ?? 0,
  retornoMatch: {
    hardSkills: item.retornoMatch?.hardSkills ?? 0,
    softSkills: item.retornoMatch?.softSkills ?? 0,
    methodologies: item.retornoMatch?.methodologies ?? 0,
    domains: item.retornoMatch?.domains ?? 0,
    languages: item.retornoMatch?.languages ?? 0,
  },
  resultadoHabilidades: mapResultadoHabilidadesFromApi(
    item.resultadoHabilidades ?? null,
    item.id || item.codigoInternoColaborador
  ),
  gestoresOperacionais: item.nomeGestorOperacional
    ? [item.nomeGestorOperacional]
    : [],
})

type LegacyResultadoHabilidade = NonNullable<
  ListaIndicadoresLideradosLegacyItem['resultadoHabilidades']
>[number]

type RawResultadoHabilidade = ListaIndicadoresRawHabilidade

const mapResultadoHabilidadesFromApi = (
  habilidades:
    | LegacyResultadoHabilidade[]
    | RawResultadoHabilidade[]
    | null
    | undefined,
  contextId?: string
): MinhaEquipeHabilidade[] | null => {
  if (!habilidades || habilidades.length === 0) return null

  return habilidades.map((habilidade, index) =>
    mapRawHabilidade(habilidade, contextId, index)
  )
}

const mapRawHabilidade = (
  raw: LegacyResultadoHabilidade | RawResultadoHabilidade,
  contextId?: string,
  index = 0
): MinhaEquipeHabilidade => {

  const label = raw.name ?? ('habilidade' in raw ? raw.habilidade : undefined) ?? 'Habilidade'
  const type = raw.type ?? ('perfilTipoId' in raw ? raw.perfilTipoId : undefined) ?? 0
  const requiredLevel = raw.requiredLevel ?? ('vagaNivel' in raw ? raw.vagaNivel : undefined) ?? ''
  const currentLevel = raw.currentLevel ?? ('colaboradorNivel' in raw ? raw.colaboradorNivel : undefined) ?? ''
  const status = 'status' in raw ? raw.status : undefined

  const sanitizedLabel = label.replace(/\s+/g, '_').toLowerCase()
  const id =
    raw.id ??
    `${contextId ?? 'habilidade'}-${sanitizedLabel}-${type}-${index}`

  return {
    id,
    name: label,
    type,
    requiredLevel,
    currentLevel,
    pendencia: raw.pendencia ?? false,
    interesse: raw.interesse ?? 0,
    status,
  }
}

const mapDetalhamentoCalculoToRetornoMatch = (
  detalhamento?: ListaIndicadoresDetalhamentoCalculo
): MinhaEquipeColaborador['retornoMatch'] => ({
  hardSkills: detalhamento?.hard_skills?.score_bruto_categoria ?? 0,
  softSkills: detalhamento?.soft_skills?.score_bruto_categoria ?? 0,
  methodologies: detalhamento?.metodologias?.score_bruto_categoria ?? 0,
  domains: detalhamento?.dominios_negocio?.score_bruto_categoria ?? 0,
  languages: detalhamento?.idiomas?.score_bruto_categoria ?? 0,
})

const createEmptyRetornoMatch = (): MinhaEquipeColaborador['retornoMatch'] => ({
  hardSkills: 0,
  softSkills: 0,
  methodologies: 0,
  domains: 0,
  languages: 0,
})

/**
 * Mapeia resposta da API para KPIs
 */
export const mapKPIsFromApi = (
  response: TotalizacaoIndicadoresResponse
): MinhaEquipeKPIs => {
  const kpi = response?.retorno?.[0]

  if (!kpi) {
    return {
      totColaboradores: 0,
      totPendentesSkills: 0,
      mediaMatch: 0,
    }
  }

  return {
    totColaboradores: kpi.totColaboradores || 0,
    totPendentesSkills: kpi.totPendentesSkills || 0,
    mediaMatch: kpi.mediaMatch || 0,
  }
}

/**
 * Mapeia resposta da API para sugestões
 * A API retorna SugestaoItem[] com historicoSugestao
 * Precisamos pegar o status mais recente do histórico
 */
export const mapSugestoesFromApi = (
  response: BuscarSugestoesResponse
): MinhaEquipeSugestao[] => {
  if (!response?.retorno) return []

  return response.retorno.map((item) => {
    // Pegar o status mais recente do histórico (primeiro item do array)
    const latestHistorico = item.historicoSugestao?.[0]
    const statusId = latestHistorico?.tbStatusSugestaoId ?? 2 // 2 = Pendente se não houver histórico

    return {
      sugestaoId: item.id,
      nomeHabilidade: item.descricaoSkill,
      perfilTipoId: item.tipo_Id,
      codigoInternoColaborador: item.codigoInternoColaborador,
      perfilId: item.perfil_Id,
      tbStatusSugestaoId: statusId,
      dataSugestao: item.data,
      observacao: latestHistorico?.observacao,
      skillId: item.skill_Id,
      nivelId: item.senioridade_Id,
      senioridadeNome: item.senioridade,
    }
  })
}

/**
 * Helper para converter timestamp (number) ou string para ISO string
 * Sempre retorna uma string ISO válida
 */
const normalizeDate = (value: string | number | undefined | null): string => {
  // Tratar 0 como valor válido (timestamp válido)
  if (value === null || value === undefined || value === '') {
    return new Date().toISOString()
  }
  
  if (typeof value === 'number') {
    // Timestamp em milissegundos
    const date = new Date(value)
    // Verificar se é uma data válida
    if (isNaN(date.getTime())) {
      return new Date().toISOString()
    }
    return date.toISOString()
  }
  
  if (typeof value === 'string') {
    // Se for formato "YYYY-MM-DD", converter para ISO usando UTC para evitar problemas de timezone
    if (/^\d{4}-\d{2}-\d{2}$/.test(value)) {
      // Usar UTC para preservar a data exata sem deslocamento de timezone
      const [year, month, day] = value.split('-').map(Number)
      const date = new Date(Date.UTC(year, month - 1, day, 0, 0, 0, 0))
      // Verificar se é uma data válida
      if (isNaN(date.getTime())) {
        return new Date().toISOString()
      }
      return date.toISOString()
    }
    
    // Tentar parsear como ISO string ou qualquer formato de data
    const date = new Date(value)
    if (isNaN(date.getTime())) {
      // Se não conseguir parsear, retornar data atual
      return new Date().toISOString()
    }
    return date.toISOString()
  }
  
  return new Date().toISOString()
}

/**
 * Mapeia status da API para status do domínio
 */
const mapStatus = (status: string | undefined): 'em_andamento' | 'concluida' | 'atrasada' => {
  if (!status) return 'em_andamento'
  
  const statusLower = status.toLowerCase()
  
  if (statusLower === 'concluida' || statusLower === 'concluída') {
    return 'concluida'
  }
  
  if (statusLower === 'atrasada') {
    return 'atrasada'
  }
  
  // "Não iniciado", "Em andamento", "em_andamento" -> em_andamento
  return 'em_andamento'
}

/**
 * Mapeia resposta da API Xano para PDIs
 */
export const mapPDIsFromApi = (response: XanoPDIResponse[]): MinhaEquipePDI[] => {
  if (!Array.isArray(response)) return []

  return response.map((item) => {
    // Extrair skill name: pode estar no topo ou no array skills
    let skillName = item.skillName || item.skill_name || ''
    
    if (!skillName && item.skills && Array.isArray(item.skills) && item.skills.length > 0) {
      skillName = item.skills[0]?.skill_name || ''
    }
    
    return {
      id: item.id || 0,
      skillName: skillName,
      deadline: normalizeDate(item.deadline || item.prazo),
      status: mapStatus(item.status),
      actions: item.actions || item.acoes || '',
      created_at: normalizeDate(item.created_at || item.createdAt),
      updated_at: normalizeDate(item.updated_at || item.updatedAt),
    }
  })
}
