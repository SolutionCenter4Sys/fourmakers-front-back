import type { MinhaJornadaSkill } from '@domain/entities/MinhaJornadaSkill'
import type { MinhaJornadaProfile } from '@domain/entities/MinhaJornadaProfile'
import type { MinhaJornadaAdherence } from '@domain/entities/MinhaJornadaAdherence'
import type { MinhaJornadaInteresse } from '@domain/entities/MinhaJornadaInteresse'
import type { MinhaJornadaSugestao } from '@domain/entities/MinhaJornadaSugestao'
import type { CriarMinhaJornadaPdiPayload } from '@domain/entities/MinhaJornadaGoal'

export interface MinhaJornadaRepository {
  // Leitura
  getPerfil360Skills(
    codColaborador: string,
    token: string,
  ): Promise<MinhaJornadaSkill[]>

  /**
   * Busca as skills do perfil de atuação em que o colaborador está alocado
   * e retorna uma estrutura flatten de perfil + habilidades.
   */
  getPerfilAtuacaoFlattened(
    codColaborador: string,
    orgId: number,
    token: string,
  ): Promise<MinhaJornadaProfile>

  /**
   * Busca todos os perfis de atuação em que o colaborador está alocado.
   */
  getPerfisAtuacao(
    codColaborador: string,
    orgId: number,
    token: string,
  ): Promise<MinhaJornadaProfile[]>

  /**
   * Obtém o cálculo de aderência oficial do backend.
   */
  getAdherence(
    codigoInternoColaborador: string,
    perfilId: string,
    token: string,
  ): Promise<MinhaJornadaAdherence>

  // Escrita
  registrarInteresseSkill(
    interesse: MinhaJornadaInteresse[],
    token: string,
  ): Promise<void>

  atualizarNivelCompetencia(
    payload: {
      id: number
      nivelId: number
      cpf: string
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      gestorExternoPerfil: string
      minhaJornada: boolean
    },
    token: string,
  ): Promise<void>

  criarPdi(payload: CriarMinhaJornadaPdiPayload): Promise<void>

  adicionarSkillPerfil360(
    payload: {
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      items: Array<{
        id: number
        descricao: string
        nivelId: number
        cpf: string
        gestorExternoPerfil: string
        minhaJornada: boolean
      }>
    },
    token: string,
  ): Promise<void>

  /**
   * Remove skill do perfil 360 do colaborador.
   * Para hard: competenciaId (id da skill no catálogo). Para soft/dominio/idioma: id. Para metodologia: codMetodologia na query.
   */
  removerSkillPerfil360(
    params: {
      tipo: 'hard' | 'soft' | 'metodologia' | 'dominio' | 'idioma'
      idOuCompetenciaId: number
      cpf: string
    },
    token: string,
  ): Promise<void>

  sugerirNovaSkill(
    payload: MinhaJornadaSugestao,
    token: string,
  ): Promise<void>

  gravarLogPdiScreenMovement(
    params: {
      token: string
      codigoInternoColaborador: string
      gestorExternoPerfil: string
      skills: MinhaJornadaSkill[]
    },
  ): Promise<void>

  buscarPDIColaborador(params: {
    token: string
    uuid_colab: string
  }): Promise<Array<{
    id: number
    skills: Array<{
      id: number
      skill_name: string
      pdi_id: number
    }>
  }>>
}



