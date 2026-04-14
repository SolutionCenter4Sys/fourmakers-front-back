/**
 * Meta de PDI (Plano de Desenvolvimento Individual) do colaborador
 */
export interface MinhaEquipePDI {
  id: number
  skillName: string
  deadline: Date | string
  status: 'em_andamento' | 'concluida' | 'atrasada'
  actions: string
  created_at: Date | string
  updated_at: Date | string
}
