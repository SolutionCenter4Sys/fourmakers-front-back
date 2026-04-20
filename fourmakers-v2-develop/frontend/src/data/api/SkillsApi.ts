import { injectable } from 'tsyringe'
import { httpClient } from './httpClient'
import type { MinhaJornadaSkillType } from '@domain/entities/MinhaJornadaSkill'

/**
 * Skill search result from API
 */
export interface SkillSearchResult {
  id: number
  nome: string
  descricao?: string
}

/**
 * Skill level result from API
 */
export interface NivelResult {
  id: number
  descricao: string
}

/**
 * Suggestion history API response
 */
export interface SugestaoApiResponse {
  sucesso: boolean
  mensagem: string
  erros: string | null
  retorno: SugestaoApiItem[]
}

export interface SugestaoApiItem {
  id: string
  codigoInternoColaborador: string
  codigoGestorAdm: string
  codigoCliente: string
  tipo_Id: number
  descricaoTipo: string
  skill_Id: number
  descricaoSkill: string
  perfil_Id: string
  senioridade_Id: number
  senioridade: string
  data: string
  ativo: boolean
  historicoSugestao?: Array<{
    id: string
    codigoInternoColaboradorAvaliador: string
    codigoInternoColaborador: string
    aprovado: boolean
    tbStatusSugestaoId: number
    perfil_Id: string
    observacao: string
    data: string
  }>
}

@injectable()
export class SkillsApi {

  /**
   * Search skills by type and search term
   */
  /**
   * Lista competências por tipo (APIs: ListarCompetencia, ListarSoftskill,
   * ListarMetodologia, ListarDominio, ListarIdiomasNaoAtribuidos).
   * @param busca termo de busca (pode ser vazio para listar todas)
   */
  async searchSkillsByType(
    token: string,
    skillType: MinhaJornadaSkillType,
    busca: string
  ): Promise<SkillSearchResult[]> {
    const config = this.getSkillConfig(skillType)
    const queryParams = new URLSearchParams()
    queryParams.set('busca', busca.trim())
    queryParams.set('cursor', '0')
    queryParams.set('limite', '20000')

    const data = await httpClient.get<any>(
      `${config.searchPath}?${queryParams.toString()}`,
      {
        token,
        headers: {
          accept: '*/*',
          'content-type': 'application/json; charset=utf-8',
        },
      }
    )

    const items = data?.[config.searchResultKey] ?? []
    if (!Array.isArray(items)) return []

    return items.map(
      (item: { id: number; descricao?: string | null; nome?: string | null }) => {
        const raw = (item.descricao ?? item.nome ?? '').toString().trim()
        return {
          id: item.id,
          nome: raw || String(item.id),
          descricao: item.descricao != null ? String(item.descricao).trim() : undefined,
        }
      }
    )
  }

  /**
   * Get skill levels by type
   */
  async getNiveisByType(
    token: string,
    skillType: MinhaJornadaSkillType
  ): Promise<NivelResult[]> {
    const config = this.getSkillConfig(skillType)
    
    const data = await httpClient.get<any>(
      config.niveisPath,
      { 
        token,
        headers: {
          accept: '*/*',
          'content-type': 'application/json; charset=utf-8',
        }
      }
    )
    
    const items: NivelResult[] = data[config.niveisResultKey] || []

    // Filter out "a definir" levels
    return items.filter(
      (n) => n.descricao.toLowerCase() !== 'a definir'
    )
  }

  /**
   * Get suggestion history for a collaborator
   */
  async getSuggestionHistory(
    token: string,
    params: {
      codInternoGestor: string
      perfilId: string
      codInternoColaborador: string
    }
  ): Promise<SugestaoApiResponse> {
    return httpClient.get<SugestaoApiResponse>(
      `/GestaoDeAlocados/MinhaJornada/BuscarSugestaoPorCodColaboradorOuAdm?codInternoGestor=${encodeURIComponent(
        params.codInternoGestor
      )}&perfilId=${encodeURIComponent(
        params.perfilId
      )}&codInternoColaborador=${encodeURIComponent(
        params.codInternoColaborador
      )}`,
      { 
        token,
        headers: {
          accept: '*/*',
          'content-type': 'application/json; charset=utf-8',
        }
      }
    )
  }

  /**
   * Get configuration for each skill type (strategy pattern replacement)
   */
  private getSkillConfig(skillType: MinhaJornadaSkillType): {
    searchPath: string
    searchResultKey: string
    niveisPath: string
    niveisResultKey: string
    label: string
  } {
    const configs: Record<
      MinhaJornadaSkillType,
      {
        searchPath: string
        searchResultKey: string
        niveisPath: string
        niveisResultKey: string
        label: string
      }
    > = {
      hard: {
        searchPath: '/api/Competencia/ListarCompetencia',
        searchResultKey: 'competencias',
        niveisPath: '/api/Competencia/ListarNivelCompetencia',
        niveisResultKey: 'niveis',
        label: 'competências',
      },
      soft: {
        searchPath: '/api/Competencia/Softskill/ListarSoftskill',
        searchResultKey: 'retorno',
        niveisPath: '/api/Competencia/Softskill/ListarNivelSoftskill',
        niveisResultKey: 'retorno',
        label: 'soft skills',
      },
      methodology: {
        searchPath: '/api/Competencia/Metodologia/ListarMetodologia',
        searchResultKey: 'retorno',
        niveisPath: '/api/Competencia/Metodologia/ListarNivelMetodologia',
        niveisResultKey: 'retorno',
        label: 'metodologias',
      },
      domain: {
        searchPath: '/api/Competencia/Dominio/ListarDominio',
        searchResultKey: 'dominio',
        niveisPath: '/api/Competencia/Dominio/ListarNivelDominio',
        niveisResultKey: 'niveis',
        label: 'domínios de negócio',
      },
      language: {
        searchPath: '/api/Competencia/Idioma/ListarIdiomasNaoAtribuidos',
        searchResultKey: 'idioma',
        niveisPath: '/api/Competencia/Idioma/ListarNivelIdioma',
        niveisResultKey: 'niveis',
        label: 'idiomas',
      },
    }

    return configs[skillType]
  }
}

