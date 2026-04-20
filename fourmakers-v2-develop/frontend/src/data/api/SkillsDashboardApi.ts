import type { SkillLogEntry } from '@domain/entities/SkillLogEntry'
import { httpClient } from './httpClient'

/**
 * Maps API tipoSkill integer values to human-readable labels
 */
const TIPO_SKILL_REVERSE_MAP: Record<number, string> = {
  1: 'Hardskill',
  3: 'Metodologia',
  4: 'Domínio de Negócio',
  8: 'Softskill',
  9: 'Idioma',
}

/**
 * Converts API tipoSkill integer to human-readable string
 */
const mapTipoSkillFromInt = (tipoSkillInt: number | string | undefined): string => {
  if (tipoSkillInt === undefined || tipoSkillInt === null) return ''
  
  const intValue = typeof tipoSkillInt === 'string' ? parseInt(tipoSkillInt, 10) : tipoSkillInt
  
  if (isNaN(intValue)) return String(tipoSkillInt)
  
  return TIPO_SKILL_REVERSE_MAP[intValue] || String(tipoSkillInt)
}

// Mock data - will be replaced with real API calls when endpoints are available
//const MOCK_SKILL_LOGS: SkillLogEntry[] = [
//   { id: 1, nome: "Ana Silva", cliente: "Google", perfil: "Frontend Dev", skill: "React", senioridade: "Senior", tipoSkill: "Hard Skill", unidade: "SP", evento: "Inserir", data: "2023-10-01" },
//   { id: 2, nome: "Carlos Souza", cliente: "Amazon", perfil: "Backend Dev", skill: "Java", senioridade: "Pleno", tipoSkill: "Hard Skill", unidade: "RJ", evento: "Sugerir", data: "2023-10-02" },
//   { id: 3, nome: "Ana Silva", cliente: "Google", perfil: "Frontend Dev", skill: "AWS", senioridade: "Senior", tipoSkill: "Hard Skill", unidade: "SP", evento: "Adicionar ao PDI", data: "2023-10-03" },
//   { id: 4, nome: "Beatriz Lima", cliente: "Meta", perfil: "Product Owner", skill: "Scrum", senioridade: "Specialist", tipoSkill: "Metodologias", unidade: "MG", evento: "Rejeitar", data: "2023-10-04" },
//   { id: 5, nome: "João Doe", cliente: "Netflix", perfil: "QA", skill: "Cypress", senioridade: "Junior", tipoSkill: "Hard Skill", unidade: "SP", evento: "Inserir", data: "2023-10-05" },
//   { id: 6, nome: "Carlos Souza", cliente: "Amazon", perfil: "Backend Dev", skill: "Python", senioridade: "Pleno", tipoSkill: "Hard Skill", unidade: "RJ", evento: "Rejeitar", data: "2023-10-05" },
//   { id: 7, nome: "Mariana Costa", cliente: "Google", perfil: "UX Designer", skill: "Figma", senioridade: "Senior", tipoSkill: "Hard Skill", unidade: "RS", evento: "Sugerir", data: "2023-10-06" },
//   { id: 8, nome: "Ana Silva", cliente: "Google", perfil: "Frontend Dev", skill: "Inglês", senioridade: "Fluente", tipoSkill: "Idiomas", unidade: "SP", evento: "Inserir", data: "2023-10-07" },
//   { id: 9, nome: "Pedro Rocha", cliente: "Microsoft", perfil: "DevOps", skill: "Docker", senioridade: "Pleno", tipoSkill: "Hard Skill", unidade: "SP", evento: "Adicionar ao PDI", data: "2023-10-08" },
//   { id: 10, nome: "Beatriz Lima", cliente: "Meta", perfil: "Product Owner", skill: "Comunicação", senioridade: "N/A", tipoSkill: "Soft Skill", unidade: "MG", evento: "Sugerir", data: "2023-10-09" },
//   { id: 11, nome: "Lucas Martins", cliente: "Netflix", perfil: "Backend Dev", skill: "Node.js", senioridade: "Senior", tipoSkill: "Hard Skill", unidade: "SP", evento: "Inserir", data: "2023-10-10" },
//   { id: 12, nome: "Fernanda Torres", cliente: "Amazon", perfil: "Data Scientist", skill: "Pandas", senioridade: "Pleno", tipoSkill: "Hard Skill", unidade: "MG", evento: "Sugerir", data: "2023-10-11" },
//   { id: 13, nome: "Lucas Martins", cliente: "Netflix", perfil: "Backend Dev", skill: "Kubernetes", senioridade: "Senior", tipoSkill: "Hard Skill", unidade: "SP", evento: "Adicionar ao PDI", data: "2023-10-12" },
//   { id: 14, nome: "Paula Ramos", cliente: "Google", perfil: "Mobile Dev", skill: "Flutter", senioridade: "Junior", tipoSkill: "Hard Skill", unidade: "RS", evento: "Inserir", data: "2023-10-13" },
//   { id: 15, nome: "Ricardo Gomes", cliente: "Microsoft", perfil: "Frontend Dev", skill: "Angular", senioridade: "Pleno", tipoSkill: "Hard Skill", unidade: "RJ", evento: "Rejeitar", data: "2023-10-14" },
// ]

// const MOCK_TOTAL_USERS = 25

export class SkillsDashboardApi {
  async getSkillLogs(_token: string, _orgId: number, limit: number = 10, cursor: number = 0): Promise<SkillLogEntry[]> {
    const token = _token;
    
    let payload: any;
    try {
      payload = await httpClient.get<any>(
        `/api/Competencia/DashboardMinhaJornada/LogDetalhado?limit=${limit}&cursor=${cursor}`,
        { token }
      );
    } catch (error) {
      // Se o erro for devido a resposta vazia, retornar array vazio
      if (error instanceof Error && error.message.includes('Erro ao processar resposta')) {
        return [];
      }
      throw error;
    }
    
    // Tratar payload nulo ou undefined
    if (payload === null || payload === undefined) {
      return [];
    }
    
    // Mapear resposta da API para entidade
    const mapItem = (item: any, index: number): SkillLogEntry => {
      // A API pode retornar tipoSkill como string ("SOFTSKILL", "IDIOMA") ou número
      // Tentar mapear primeiro como número, depois usar string diretamente se não for número
      let tipoSkillMapped = ''
      if (item.tipoSkill !== undefined && item.tipoSkill !== null) {
        const tipoSkillValue = item.tipoSkill
        // Se for número, usar o mapeamento reverso
        if (typeof tipoSkillValue === 'number' || (typeof tipoSkillValue === 'string' && !isNaN(Number(tipoSkillValue)))) {
          tipoSkillMapped = mapTipoSkillFromInt(tipoSkillValue)
        } else {
          // Se for string direta, usar como está (ex: "SOFTSKILL", "IDIOMA")
          tipoSkillMapped = String(tipoSkillValue)
        }
      }

      return {
        id: cursor + index + 1,
        nome: item.nomeColaborador || '',
        cliente: item.nomeCliente || '',
        perfil: item.nomePerfil || '',
        skill: item.nomeSkill || '',
        senioridade: item.senioridade || '',
        tipoSkill: tipoSkillMapped,
        unidade: '',
        evento: item.evento || 'Inserir',
        data: '',
      }
    }
    
    // Tratar payload.retorno ou payload direto
    if (payload?.retorno !== undefined) {
      if (Array.isArray(payload.retorno)) {
        // Array vazio é válido, não é erro
        return payload.retorno.map(mapItem) as SkillLogEntry[];
      }
      throw new Error('Formato de resposta inválido: retorno não é um array');
    }
    
    if (Array.isArray(payload)) {
      // Array vazio é válido, não é erro
      return payload.map(mapItem) as SkillLogEntry[];
    }
    
    throw new Error('Formato de resposta inválido: payload não é um array ou não contém retorno');
  }
  async getBigNumbers(_token: string, _orgId: number): Promise<{skillsAdicionadas:number; skillsSugeridas:number; adicionadasPDI:number; skillsRejeitadas:number}> {
    const token = _token;
    const payload = await httpClient.get<any>(
      '/api/Competencia/DashboardMinhaJornada/BigNumbers',
      { token }
    );
    const retorno = payload?.retorno ?? payload;
    return {
      skillsAdicionadas: retorno?.skillsAdicionadas ?? 0,
      skillsSugeridas: retorno?.skillsSugeridas ?? 0,
      adicionadasPDI: retorno?.adicionadasPDI ?? 0,
      skillsRejeitadas: retorno?.skillsRejeitadas ?? 0,
    };
  }
  async getTopDez(_token: string, _orgId: number): Promise<any> {
    const token = _token;
    const payload = await httpClient.get<any>(
      '/api/Competencia/DashboardMinhaJornada/TopDez',
      { token }
    );
    const nested = payload?.retorno?.topDezPorMovimentacao ?? payload?.topDezPorMovimentacao ?? payload;
    // Wrap to match TopDezData contract: { topDezPorMovimentacao: ... }
    return { topDezPorMovimentacao: nested };
  }
  // Removed duplicate legacy mock getSkillLogs implementation

  /**
   * Fetches total number of users in the system
   * TODO: Replace with real API call when endpoint is available
   * @param token - Authentication token
   * @param orgId - Organization ID
   * @returns Promise with total users count
   */
  async getTotalUsers(_token: string, _orgId: number): Promise<number> {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 200))
    
    // TODO: Replace with actual API call
    // const response = await fetch(`${API_BASE_URL}/api/skills-dashboard/total-users`, {
    //   method: 'GET',
    //   headers: {
    //     'Content-Type': 'application/json',
    //     'Authorization': `Bearer ${token}`,
    //   },
    // })
    // if (!response.ok) throw new Error('Failed to fetch total users')
    // const data = await response.json()
    // return data.totalUsers
    
    //return Promise.resolve(MOCK_TOTAL_USERS)
    return 0;
  }
}

