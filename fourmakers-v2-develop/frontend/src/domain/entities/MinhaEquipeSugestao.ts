/**
 * Sugestão de habilidade enviada por colaborador
 */
export interface MinhaEquipeSugestao {
  sugestaoId: string // UUID
  nomeHabilidade: string
  perfilTipoId: number // 1-5
  codigoInternoColaborador: string
  perfilId: string
  tbStatusSugestaoId: number // 1=Aprovado, 2=Pendente, 3=Rejeitado
  dataSugestao: Date | string
  observacao?: string
  skillId: number // ID da skill obtida pelo endpoint GET
  nivelId: number // ID do nível de senioridade obtida pelo endpoint GET
  senioridadeNome: string // Nome da senioridade
}

/**
 * Payload para aprovar ou rejeitar uma sugestão
 */
export interface AprovarRejeitarSugestaoPayload {
  sugestaoId: string
  codigoInternoColaborador: string
  aprovado: boolean
  tbStatusSugestaoId: number // 1 ou 3
  perfil_Id: string
  observacao: string // Obrigatório
  skillId: number // ID da skill
  nivelId: number // ID do nível de senioridade
  itemPerfil: number // Código do tipo de skill (1=hard, 8=soft, 3=methodology, 4=domain, 9=language)
}
