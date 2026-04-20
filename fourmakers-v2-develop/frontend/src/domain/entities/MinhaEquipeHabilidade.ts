/**
 * Representa uma habilidade de um colaborador na visão de Minha Equipe
 */
export interface MinhaEquipeHabilidade {
  id: string
  name: string
  type: number // perfilTipoId: 1-5 (Hard, Soft, Methodology, Domain, Language)
  requiredLevel: string | number
  currentLevel: string | number
  pendencia: boolean // true=pendente, false=adicionada/rejeitada
  interesse: number // 1=interessado, 0=rejeitado
  status?: string
}
