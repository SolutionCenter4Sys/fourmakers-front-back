export interface PerfilColaboradorCampanhaValidationData {
  paisSelecionado: string
  bairro: string
  modeloTrabalho: string
  localTrabalho: string
  clienteSelecionado: boolean
  frequencia: string
  frequenciaId: number
  diasSemana: string[]
  possuiCertificadoAWS: string
  certificadosAWS: {
    technical: { status: string }
    foundational: { status: string }
    accredited: { status: string }
  }
  comprovanteFile: File | null
}

export interface ValidationError {
  message: string
}

